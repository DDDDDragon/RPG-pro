using System.Text.RegularExpressions;
using System.Text;

namespace RPG_pro.Basics.Mods
{
    internal class BuildProperties
    {
        public IEnumerable<ModReference> Refs(bool includeWeak)
        {
            if (!includeWeak)
            {
                return modReferences;
            }
            return modReferences.Concat(this.weakReferences);
        }
        public IEnumerable<string> RefNames(bool includeWeak) => from dep in Refs(includeWeak)select dep.mod;
        private static IEnumerable<string> ReadList(string value) => from s in value.Split(',', StringSplitOptions.None) select s.Trim() into s where s.Length > 0 select s;
        private static IEnumerable<string> ReadList(BinaryReader reader)
        {
            List<string> list = new();
            string item = reader.ReadString();
            while (item.Length > 0)
            {
                list.Add(item);
                item = reader.ReadString();
            }
            return list;
        }
        private static void WriteList<T>(IEnumerable<T> list, BinaryWriter writer)
        {
            foreach (T item in list)
            {
                writer.Write(item.ToString());
            }
            writer.Write("");
        }
        internal static BuildProperties ReadBuildFile(string modDir)
        {
            string propertiesFile = modDir + Path.DirectorySeparatorChar.ToString() + "build.txt";
            string descriptionfile = modDir + Path.DirectorySeparatorChar.ToString() + "description.txt";
            BuildProperties properties = new();
            if (!File.Exists(propertiesFile))
            {
                return properties;
            }
            if (File.Exists(descriptionfile))
            {
                properties.description = File.ReadAllText(descriptionfile);
            }
            foreach (string line in File.ReadAllLines(propertiesFile))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    int split = line.IndexOf('=');
                    string property = line.Substring(0, split).Trim();
                    string value = line.Substring(split + 1).Trim();
                    if (value.Length != 0)
                    {
                        if (property == nameof(side))
                        {
                            if (!Enum.TryParse(value, true, out properties.side))
                            {
                                throw new Exception("side is not one of (Both, Client, Server, NoSync): " + value);
                            }
                        }
                        if (property == nameof(sortAfter))
                        {
                            properties.sortAfter = ReadList(value).ToArray();
                        }
                        if (property == nameof(weakReferences))
                        {
                            properties.weakReferences = ReadList(value).Select(new Func<string, ModReference>(ModReference.Parse)).ToArray();
                        }
                        if (property == nameof(playableOnPreview))
                        {
                            properties.playableOnPreview = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                        }
                        if (property == nameof(dllReferences))
                        {
                            properties.dllReferences = ReadList(value).ToArray();
                        }
                        if (property == nameof(includeSource))
                        {
                            properties.includeSource = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                        }
                        if (property == nameof(version))
                        {
                            properties.version = new Version(value);
                        }
                        if (property == nameof(buildIgnores))
                        {
                            properties.buildIgnores = (from s in value.Split(',', StringSplitOptions.None) select s.Trim().Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar) into s where s.Length > 0 select s).ToArray();
                        }
                        if (property == nameof(author))
                        {
                            properties.author = value;
                        }
                        if (property == nameof(hideCode))
                        {
                            properties.hideCode = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                        }
                        if (property == nameof(sortBefore))
                        {
                            properties.sortBefore = ReadList(value).ToArray();
                        }
                        if (property == nameof(homepage))
                        {
                            properties.homepage = value;
                        }
                        if (property == nameof(hideResources))
                        {
                            properties.hideResources = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                        }
                        if (property == nameof(modReferences))
                        {
                            properties.modReferences = ReadList(value).Select(new Func<string, ModReference>(ModReference.Parse)).ToArray();
                        }
                        if (property == nameof(displayName))
                        {
                            properties.displayName = value;
                        }
                    }
                }
            }
            var refs = properties.RefNames(true).ToList();
            if (refs.Count != refs.Distinct().Count())
            {
                throw new Exception("Duplicate mod/weak reference");
            }
            properties.sortAfter = (from dep in properties.RefNames(true)
                                    where !properties.sortBefore.Contains(dep)
                                    select dep).Concat(properties.sortAfter).Distinct().ToArray();
            return properties;
        }
        internal byte[] ToBytes()
        {
            byte[] data;
            using (MemoryStream memoryStream = new())
            {
                using (BinaryWriter writer = new(memoryStream))
                {
                    if (dllReferences.Length != 0)
                    {
                        writer.Write(nameof(dllReferences));
                        WriteList(this.dllReferences, writer);
                    }
                    if (modReferences.Length != 0)
                    {
                        writer.Write(nameof(modReferences));
                        WriteList(modReferences, writer);
                    }
                    if (weakReferences.Length != 0)
                    {
                        writer.Write(nameof(weakReferences));
                        WriteList(weakReferences, writer);
                    }
                    if (sortAfter.Length != 0)
                    {
                        writer.Write(nameof(sortAfter));
                        WriteList(sortAfter, writer);
                    }
                    if (sortBefore.Length != 0)
                    {
                        writer.Write(nameof(sortBefore));
                        WriteList(sortBefore, writer);
                    }
                    if (author.Length > 0)
                    {
                        writer.Write(nameof(author));
                        writer.Write(author);
                    }
                    writer.Write(nameof(version));
                    writer.Write(version.ToString());
                    if (displayName.Length > 0)
                    {
                        writer.Write(nameof(displayName));
                        writer.Write(displayName);
                    }
                    if (homepage.Length > 0)
                    {
                        writer.Write(nameof(homepage));
                        writer.Write(homepage);
                    }
                    if (description.Length > 0)
                    {
                        writer.Write(nameof(description));
                        writer.Write(description);
                    }
                    if (noCompile)
                    {
                        writer.Write("noCompile");
                    }
                    if (!playableOnPreview)
                    {
                        writer.Write("!playableOnPreview");
                    }
                    if (!hideCode)
                    {
                        writer.Write("!hideCode");
                    }
                    if (!hideResources)
                    {
                        writer.Write("!hideResources");
                    }
                    if (includeSource)
                    {
                        writer.Write(nameof(includeSource));
                    }
                    if (eacPath.Length > 0)
                    {
                        writer.Write(nameof(eacPath));
                        writer.Write(eacPath);
                    }
                    if (this.side != ModSide.Both)
                    {
                        writer.Write(nameof(side));
                        writer.Write((byte)this.side);
                    }
                    writer.Write(nameof(buildVersion));
                    writer.Write(this.buildVersion.ToString());
                    writer.Write("");
                }
                data = memoryStream.ToArray();
            }
            return data;
        }
        internal static BuildProperties ReadModFile(ModFile modFile)
        {
            return ReadFromStream(modFile.GetStream("Info", false));
        }
        internal static BuildProperties ReadFromStream(Stream stream)
        {
            BuildProperties properties = new()
            {
                hideCode = true,
                hideResources = true
            };
            using (BinaryReader reader = new(stream))
            {
                string tag = reader.ReadString();
                while (tag.Length > 0)
                {
                    if (tag == nameof(dllReferences))
                    {
                        properties.dllReferences = ReadList(reader).ToArray<string>();
                    }
                    if (tag == nameof(modReferences))
                    {
                        properties.modReferences = ReadList(reader).Select(new Func<string, ModReference>(ModReference.Parse)).ToArray();
                    }
                    if (tag == nameof(weakReferences))
                    {
                        properties.weakReferences = ReadList(reader).Select(new Func<string, ModReference>(ModReference.Parse)).ToArray();
                    }
                    if (tag == nameof(sortAfter))
                    {
                        properties.sortAfter = ReadList(reader).ToArray();
                    }
                    if (tag == nameof(sortBefore))
                    {
                        properties.sortBefore = ReadList(reader).ToArray();
                    }
                    if (tag == nameof(author))
                    {
                        properties.author = reader.ReadString();
                    }
                    if (tag == nameof(version))
                    {
                        properties.version = new Version(reader.ReadString());
                    }
                    if (tag == nameof(displayName))
                    {
                        properties.displayName = reader.ReadString();
                    }
                    if (tag == nameof(homepage))
                    {
                        properties.homepage = reader.ReadString();
                    }
                    if (tag == nameof(description))
                    {
                        properties.description = reader.ReadString();
                    }
                    if (tag == "noCompile")
                    {
                        properties.noCompile = true;
                    }
                    if (tag == "!playableOnPreview")
                    {
                        properties.playableOnPreview = false;
                    }
                    if (tag == "!hideCode")
                    {
                        properties.hideCode = false;
                    }
                    if (tag == "!hideResources")
                    {
                        properties.hideResources = false;
                    }
                    if (tag == nameof(includeSource))
                    {
                        properties.includeSource = true;
                    }
                    if (tag == nameof(eacPath))
                    {
                        properties.eacPath = reader.ReadString();
                    }
                    if (tag == nameof(side))
                    {
                        properties.side = (ModSide)reader.ReadByte();
                    }
                    if (tag == nameof(buildVersion))
                    {
                        properties.buildVersion = new Version(reader.ReadString());
                    }
                    tag = reader.ReadString();
                }
            }
            return properties;
        }
        internal static void InfoToBuildTxt(Stream src, Stream dst)
        {
            BuildProperties properties = ReadFromStream(src);
            StringBuilder sb = new();
            StringBuilder stringBuilder;
            StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler;
            if (properties.displayName.Length > 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder2 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("displayName = ");
                appendInterpolatedStringHandler.AppendFormatted(properties.displayName);
                stringBuilder2.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.author.Length > 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder3 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("author = ");
                appendInterpolatedStringHandler.AppendFormatted(properties.author);
                stringBuilder3.AppendLine(ref appendInterpolatedStringHandler);
            }
            stringBuilder = sb;
            StringBuilder stringBuilder4 = stringBuilder;
            appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder);
            appendInterpolatedStringHandler.AppendLiteral("version = ");
            appendInterpolatedStringHandler.AppendFormatted<Version>(properties.version);
            stringBuilder4.AppendLine(ref appendInterpolatedStringHandler);
            if (properties.homepage.Length > 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder5 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("homepage = ");
                appendInterpolatedStringHandler.AppendFormatted(properties.homepage);
                stringBuilder5.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.dllReferences.Length != 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder6 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("dllReferences = ");
                appendInterpolatedStringHandler.AppendFormatted(string.Join(", ", properties.dllReferences));
                stringBuilder6.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.modReferences.Length != 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder7 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("modReferences = ");
                appendInterpolatedStringHandler.AppendFormatted(string.Join<ModReference>(", ", properties.modReferences));
                stringBuilder7.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.weakReferences.Length != 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder8 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("weakReferences = ");
                appendInterpolatedStringHandler.AppendFormatted(string.Join<ModReference>(", ", properties.weakReferences));
                stringBuilder8.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.noCompile)
            {
                sb.AppendLine("noCompile = true");
            }
            if (properties.hideCode)
            {
                sb.AppendLine("hideCode = true");
            }
            if (properties.hideResources)
            {
                sb.AppendLine("hideResources = true");
            }
            if (properties.includeSource)
            {
                sb.AppendLine("includeSource = true");
            }
            if (!properties.playableOnPreview)
            {
                sb.AppendLine("playableOnPreview = false");
            }
            if (properties.side != ModSide.Both)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder9 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("side = ");
                appendInterpolatedStringHandler.AppendFormatted(properties.side);
                stringBuilder9.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.sortAfter.Length != 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder10 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("sortAfter = ");
                appendInterpolatedStringHandler.AppendFormatted(string.Join(", ", properties.sortAfter));
                stringBuilder10.AppendLine(ref appendInterpolatedStringHandler);
            }
            if (properties.sortBefore.Length != 0)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder11 = stringBuilder;
                appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder);
                appendInterpolatedStringHandler.AppendLiteral("sortBefore = ");
                appendInterpolatedStringHandler.AppendFormatted(string.Join(", ", properties.sortBefore));
                stringBuilder11.AppendLine(ref appendInterpolatedStringHandler);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            dst.Write(bytes, 0, bytes.Length);
        }
        internal bool IgnoreFile(string resource) => buildIgnores.Any((string fileMask) => FitsMask(resource, fileMask));
        private static bool FitsMask(string fileName, string fileMask) => new Regex("^" + Regex.Escape(fileMask.Replace(".", "__DOT__").Replace("*", "__STAR__").Replace("?", "__QM__")).Replace("__DOT__", "[.]").Replace("__STAR__", ".*").Replace("__QM__", ".") + "$", RegexOptions.IgnoreCase).IsMatch(fileName);
        internal string[] dllReferences = Array.Empty<string>();
        internal ModReference[] modReferences = Array.Empty<ModReference>();
        internal ModReference[] weakReferences = Array.Empty<ModReference>();
        internal string[] sortAfter = Array.Empty<string>();
        internal string[] sortBefore = Array.Empty<string>();
        internal string[] buildIgnores = Array.Empty<string>();
        internal string author = "";
        internal Version version = new(1, 0);
        internal string displayName = "";
        internal bool noCompile;
        internal bool hideCode;
        internal bool hideResources;
        internal bool includeSource;
        internal string eacPath = "";
        internal bool beta;
        internal Version buildVersion = GameInfos.GameVersion;
        internal string homepage = "";
        internal string description = "";
        internal ModSide side;
        internal bool playableOnPreview = true;
        internal struct ModReference
        {
            public ModReference(string mod, Version target)
            {
                this.mod = mod;
                this.target = target;
            }
            public override string ToString()
            {
                if (target is not null)
                {
                    string str = mod;
                    string str2 = "@";
                    Version version = target;
                    return str + str2 + (version?.ToString());
                }
                return mod;
            }
            public static ModReference Parse(string spec)
            {
                string[] split = spec.Split('@', StringSplitOptions.None);
                if (split.Length == 1)
                {
                    return new ModReference(split[0], null);
                }
                if (split.Length > 2)
                {
                    throw new Exception("Invalid mod reference: " + spec);
                }
                ModReference result;
                try
                {
                    result = new ModReference(split[0], new Version(split[1]));
                }
                catch
                {
                    throw new Exception("Invalid mod reference: " + spec);
                }
                return result;
            }
            public string mod;
            public Version target;
        }
        public enum ModSide
        {
            Both,
            Client,
            Server,
            NoSync
        }
    }
}