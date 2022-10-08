using RPG_pro.Basics.Exceptions;
using RPG_pro.Basics.Extensions;
using RPG_pro.Basics.Interfaces;
using RPG_pro.Basics.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection.Emit;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    public class ModFile
    {
        internal ModFile(string path)
        {
            FilePath = path;
        }
        public readonly string FilePath;
        public string Name { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public Version ModVersion { get; private set; } = new Version();
        public Version BuildVersion { get; private set; } = GameInfos.GameVersion;
        public Version RequestGameVersion { get; private set; } = null;
        internal bool Damaged { get; private set; } = false;
        private Stream Stream;
        private readonly Dictionary<string, FileEntry> files = new();
        public IDisposable Open()
        {
            if (Stream is not null)
            {
                throw new IOException("Open must called after Close");
            }
            try
            {
                Stream = new MemoryStream(File.ReadAllBytes(FilePath));
                Load();
            }
            catch
            {
                try
                {
                    Damaged = true;
                    Close();
                }
                catch
                {

                }
            }
            return new Utils_Normal.DisposeWrapper(Close);
        }
        public bool TryGetFileBytes(string filename, out byte[] bytes)
        {
            filename = FixPath(filename);
            if (files.TryGetValue(filename, out FileEntry entry))
            {
                if (entry.Cache is null)
                {
                    Stream.Position = entry.Offset;
                    entry.Cache = Stream.ReadBytes(entry.Length);
                }
                bytes = new byte[entry.Length];
                entry.Cache.CopyTo(bytes, 0);
                return true;
            }
            bytes = Array.Empty<byte>();
            return false;
        }
        public Stream GetFileStream(string filename)
        {
            if (TryGetFileBytes(filename, out byte[] bytes))
            {
                return new MemoryStream(bytes);
            }
            throw new IOException($"Can't find {filename}.");
        }
        public bool HasFile(string filename)
        {
            return files.ContainsKey(FixPath(filename));
        }
        public void ClearCaches() => files.Values.ForEach(FileEntry.ClearCache);
        private void Load()
        {
            BinaryReader reader = new(Stream);
            Name = reader.ReadString();
            DisplayName = reader.ReadString();
            ModVersion = new(reader.ReadString());
            BuildVersion = new(reader.ReadString());
            string s = reader.ReadString();
            if (s != "NoRequest")
            {
                RequestGameVersion = null;
            }
            ClearCaches();
            files.Clear();
            uint count = reader.ReadUInt32();
            for (uint i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                uint length = reader.ReadUInt32();
                FileEntry entry = new(name, length)
                {
                    Offset = Stream.Position
                };
                files[name] = entry;
                Stream.Position += length;
            }
        }
        internal void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            FileStream fs = File.Create(FilePath);
            BinaryWriter writer = new(fs);
            writer.Write(Name);
            writer.Write(DisplayName);
            writer.Write(ModVersion.ToString());
            writer.Write((BuildVersion = GameInfos.GameVersion).ToString());
            writer.Write(RequestGameVersion?.ToString() ?? "NoRequest");
            writer.Write((uint)files.Count);
            foreach (var entry in files.Values)
            {
                writer.Write(entry.Name);
                writer.Write(entry.Length);
                writer.Write(entry.Cache);
            }
        }
        internal void AddFile(byte[] bytes, string filename)
        {
            filename = FixPath(filename);
            FileEntry entry = new(filename, (uint)bytes.Length)
            {
                Cache = new byte[bytes.Length]
            };
            bytes.CopyTo(entry.Cache, 0);
            files[filename] = entry;
        }
        internal void RemoveFile(string filename)
        {
            filename = FixPath(filename);
            if (files.ContainsKey(filename))
            {
                files[filename].Cache = null;
                files.Remove(filename);
            }
        }
        private void Close()
        {
            if (Stream is null)
            {
                return;
            }
            Stream.Dispose();
            ClearCaches();
            files.Clear();
            Stream = null;
        }
        private static string FixPath(string filepath) => filepath.Replace("\\", "/");
        public class FileEntry
        {
            internal FileEntry(string name, uint length)
            {
                Name = name;
                Length = length;
            }
            public string Name { get; internal set; }
            public uint Length { get; internal set; }
            public byte[] Cache { get; internal set; }
            public long Offset { get; internal set; }
            public static void ClearCache(FileEntry entry) => entry.Cache = null;
        }
    }
}