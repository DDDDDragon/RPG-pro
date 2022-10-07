using RPG_pro.Basics.Exceptions;
using RPG_pro.Basics.Extensions;
using RPG_pro.Basics.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    public class ModFile : IEnumerable<ModFile.FileEntry>, IEnumerable
    {
        internal ModFile(string path, string name = null, Version version = null)
        {
            Path = path;
            Name = name;
            ModVersion = version;
        }
        public readonly string Path;
        private FileEntry[] fileTable;
        private Dictionary<string, FileEntry> files = new();
        private Stream fileStream;
        private uint openCounter;
        private EntryReadStream sharedEntryReadStream;
        private readonly List<EntryReadStream> independentEntryReadStreams = new();
        public string Name { get; private set; }
        public Version BuildVersion { get; private set; }
        public Version RequestGameVersion { get; private set; }
        public Version ModVersion { get; private set; }
        public byte[] Hash { get; private set; }
        internal byte[] Signature { get; private set; }
        public bool IsLoading() => RequestGameVersion?.Equals(GameInfos.GameVersion) ?? true;
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void CacheFiles(ISet<string> skip = null)
        {
            fileStream.Seek(fileTable[0].Offset, SeekOrigin.Begin);
            foreach (FileEntry f in fileTable)
            {
                if (f.CompressedLength > 131072L || (skip != null && skip.Contains(f.Name)))
                {
                    fileStream.Seek(f.CompressedLength, SeekOrigin.Current);
                }
                else
                {
                    f.CachedBytes = fileStream.ReadBytes(f.CompressedLength);
                }
            }
        }
        public byte[] GetBytes(FileEntry entry)
        {
            if (entry.CachedBytes != null && !entry.IsCompressed)
            {
                return entry.CachedBytes;
            }
            byte[] result;
            using (Stream stream = GetStream(entry, false))
            {
                result = stream.ReadBytes(entry.Length);
            }
            return result;
        }
        public byte[] GetBytes(string fileName)
        {
            if (files.TryGetValue(Sanitize(fileName), out FileEntry entry))
            {
                return GetBytes(entry);
            }
            return null;
        }
        public List<string> GetFileNames() => files.Keys.ToList();
        public Stream GetStream(FileEntry entry, bool newFileStream = false)
        {
            Stream stream;
            if (entry.CachedBytes != null)
            {
                stream = new MemoryStream(entry.CachedBytes);
            }
            else
            {
                if (fileStream == null)
                {
                    throw new IOException("File not open: " + Path);
                }
                if (newFileStream)
                {
                    EntryReadStream ers = new(this, entry, File.OpenRead(Path), false);
                    lock (independentEntryReadStreams)
                    {
                        independentEntryReadStreams.Add(ers);
                    }
                    stream = ers;
                }
                else
                {
                    if (sharedEntryReadStream != null)
                    {
                        throw new IOException("Previous entry read stream not closed: " + sharedEntryReadStream.Name);
                    }
                    stream = sharedEntryReadStream = new EntryReadStream(this, entry, fileStream, true);
                }
            }
            if (entry.IsCompressed)
            {
                stream = new DeflateStream(stream, CompressionMode.Decompress);
            }
            return stream;
        }
        public Stream GetStream(string fileName, bool newFileStream = false)
        {
            if (files.TryGetValue(Sanitize(fileName), out FileEntry entry))
            {
                return GetStream(entry, newFileStream);
            }
            throw new KeyNotFoundException(fileName);
        }
        public bool HasFile(string fileName) => files.ContainsKey(Sanitize(fileName));
        public void RemoveFromCache(IEnumerable<string> fileNames)
        {
            foreach (string fileName in fileNames)
            {
                files[fileName].CachedBytes = null;
            }
        }
        public void ResetCache()
        {
            for (int i = 0; i < fileTable.Length; i++)
            {
                fileTable[i].CachedBytes = null;
            }
        }
        public IEnumerator<FileEntry> GetEnumerator()
        {
            foreach (FileEntry entry in this.fileTable)
            {
                yield return entry;
            }
            yield break;
        }
        public IDisposable Open()
        {
            if (openCounter++ == 0)
            {
                try
                {
                    if (Name is null)
                    {
                        Read();
                    }
                    else
                    {
                        Check();
                    }
                }
                catch (IOException ioexception)
                {
                    if (fileStream is not null)
                    {
                        fileStream.Dispose();
                        fileStream = null;
                    }
                    Logger.Error(ioexception);
                }
            }
            return new DisposeWrapper(Close);
        }
        internal void OnStreamClosed(EntryReadStream stream)
        {
            if (stream == sharedEntryReadStream)
            {
                sharedEntryReadStream = null;
                return;
            }
            List<EntryReadStream> obj = independentEntryReadStreams;
            lock (obj)
            {
                if (!independentEntryReadStreams.Remove(stream))
                {
                    throw new IOException("Closed EntryReadStream not associated with this file. " + stream.Name + " @ " + Path);
                }
            }
        }
        internal void AddFile(string fileName, byte[] data)
        {
            fileName = Sanitize(fileName);
            int size = data.Length;
            if (size > 1024L && ShouldCompress(fileName))
            {
                using (MemoryStream ms = new(data.Length))
                {
                    using (DeflateStream ds = new(ms, CompressionMode.Compress))
                    {
                        ds.Write(data, 0, data.Length);
                    }
                    byte[] compressed = ms.ToArray();
                    if (compressed.Length < (float)size * 0.9f)
                    {
                        data = compressed;
                    }
                }
            }
            lock (files)
            {
                files[fileName] = new FileEntry(fileName, -1, size, data.Length, data);
            }
            fileTable = null;
        }
        internal void RemoveFile(string fileName)
        {
            files.Remove(Sanitize(fileName));
            fileTable = null;
        }
        internal void Save()
        {
            if (fileStream is not null)
            {
                throw new IOException("File already open: " + Path);
            }
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            using (fileStream = File.Create(Path))
            {
                using (BinaryWriter writer = new BinaryWriter(this.fileStream))
                {
                    writer.Write(Encoding.ASCII.GetBytes("RPG"));
                    writer.Write((BuildVersion = GameInfos.GameVersion).ToString());
                    int hashPos = (int)fileStream.Position;
                    writer.Write(new byte[280]);
                    int dataPos = (int)fileStream.Position;
                    writer.Write(Name);
                    writer.Write(ModVersion.ToString());
                    fileTable = files.Values.ToArray();
                    writer.Write(fileTable.Length);
                    foreach (FileEntry f in fileTable)
                    {
                        if (f.CompressedLength != f.CachedBytes.Length)
                        {
                            throw new Exception($"CompressedLength ({f.CompressedLength}) != cachedBytes.Length ({f.CachedBytes.Length}): {f.Name}");
                        }
                        writer.Write(f.Name);
                        writer.Write(f.Length);
                        writer.Write(f.CompressedLength);
                    }
                    int offset = (int)fileStream.Position;
                    foreach (FileEntry f2 in fileTable)
                    {
                        writer.Write(f2.CachedBytes);
                        f2.Offset = offset;
                        offset += f2.CompressedLength;
                    }
                    fileStream.Position = dataPos;
                    Hash = SHA1.Create().ComputeHash(fileStream);
                    fileStream.Position = hashPos;
                    writer.Write(Hash);
                    fileStream.Seek(256L, SeekOrigin.Current);
                    writer.Write((int)(fileStream.Length - dataPos));
                }
            }
            this.fileStream = null;
        }
        private void Read()
        {
            fileStream = File.OpenRead(Path);
            BinaryReader reader = new(fileStream);
            if (Encoding.ASCII.GetString(reader.ReadBytes(3)) != "RPG")
            {
                throw new IOException($"Unknow file:{Path}");
            }
            BuildVersion = new(reader.ReadString());
            Hash = reader.ReadBytes(20);
            Signature = reader.ReadBytes(256);
            reader.ReadInt32();
            long pos = fileStream.Position;
            if (!SHA1.Create().ComputeHash(fileStream).SequenceEqual(Hash))
            {
                throw new IOException("The verification code is inconsistent. The file has been modified");
            }
            fileStream.Position = pos;
            Name = reader.ReadString();
            ModVersion = new(reader.ReadString());
            int offset = 0;
            fileTable = new FileEntry[reader.ReadUInt16()];
            for (int i = 0; i < fileTable.Length; i++)
            {
                FileEntry file = new(reader.ReadString(), offset, reader.ReadInt32(), reader.ReadInt32(), null);
                fileTable[i] = files[file.Name] = file;
                offset += file.CompressedLength;
            }
            int filesstart = (int)fileStream.Position;
            fileTable.ForEach(entry =>
            {
                entry.Offset += filesstart;
            });
        }
        private void Check()
        {
            fileStream = File.OpenRead(Path);
            BinaryReader reader = new(fileStream);
            if (Encoding.ASCII.GetString(reader.ReadBytes(3)) != "RPG")
            {
                throw new IOException($"Unknow file:{Path}");
            }
            reader.ReadString();
            if (Hash != reader.ReadBytes(20))
            {
                throw new IOException($"The file has been modified:{Path}");
            }
        }
        private void Close()
        {
            if (openCounter < 1)
            {
                return;
            }
            if (--openCounter == 0)
            {
                if (sharedEntryReadStream is not null)
                {
                    throw new IOException("Previous entry read stream not closed: " + sharedEntryReadStream.Name);
                }
                if (independentEntryReadStreams.Count != 0)
                {
                    throw new IOException("Shared entry read streams not closed: " + string.Join(", ", from s in independentEntryReadStreams select s.Name));
                }
                if (fileStream is not null)
                {
                    fileStream.Close();
                    fileStream = null;
                }
            }
        }
        private static string Sanitize(string path) => path.Replace('\\', '/');
        private static bool ShouldCompress(string fileName) => !CompressExtension.Any(extension => fileName.EndsWith(extension));
        private static List<string> CompressExtension = new()
        {
            ".jpg",
            ".png",
            ".gif",
            ".mp3",
            ".mp4",
            ".ogg"
        };
        public class FileEntry
        {
            public string Name { get; }
            public int Offset { get; internal set; }
            public int Length { get; }
            public int CompressedLength { get; }
            internal FileEntry(string name, int offset, int length, int compressedLength, byte[] cachedBytes = null)
            {
                Name = name;
                Offset = offset;
                Length = length;
                CompressedLength = compressedLength;
                CachedBytes = cachedBytes;
            }
            public bool IsCompressed => Length != CompressedLength;
            internal byte[] CachedBytes;
        }
        private class DisposeWrapper : IDisposable
        {
            public DisposeWrapper(Action dispose)
            {
                this.dispose = dispose;
            }
            public void Dispose()
            {
                dispose?.Invoke();
            }
            private readonly Action dispose;
        }
    }
}
