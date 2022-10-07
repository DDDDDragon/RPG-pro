using RPG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    internal class LocalMod
    {
        public string Name => modFile.Name;
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(properties.displayName))
                {
                    return properties.displayName;
                }
                return Name;
            }
        }
        public Version BuildVersion => properties.buildVersion;
        public bool Enabled
        {
            get
            {
                return ModContent.ModsEnabled[Name];
            }
            set
            {
                ModContent.ModsEnabled[Name] = value;
            }
        }
        public override string ToString() => Name;
        public LocalMod(ModFile modFile, BuildProperties properties)
        {
            this.modFile = modFile;
            this.properties = properties;
        }
        public LocalMod(ModFile modFile) : this(modFile, BuildProperties.ReadModFile(modFile))
        {
        }
        public readonly ModFile modFile;
        public readonly BuildProperties properties;
        public DateTime lastModified;
    }
}
