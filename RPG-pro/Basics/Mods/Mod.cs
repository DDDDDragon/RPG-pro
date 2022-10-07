using RPG_pro.Basics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    public abstract class Mod : IModType
    {
        internal Mod(ModFile modFile)
        {
            File = modFile;
            Name = modFile.Name;
            FullName = modFile.Name + modFile.ModVersion.ToString();
            Version = modFile.ModVersion;
        }
        public string Name { get; internal set; }
        public string FullName { get; internal set; }
        public ModFile File { get; internal set; }
        public Version Version { get; internal set; }
        Mod IModType.Mod => this;
        public virtual void Dispose() => GC.SuppressFinalize(this);
        public bool IsLoading() => true;
        public virtual void Load() { }
        public virtual void Unload() { }
    }
}
