using RPG;
using RPG_pro.Basics.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Interfaces
{
    public interface IModType : ILoadable
    {
        public Mod Mod { get; internal set; }
        public int Index { get; internal set; }
        public virtual void SetStaticDefaults() { }
        public virtual void SetDefaults() { }
        protected virtual void Register() { }
    }
    public abstract class ModType : IModType
    {
        private Mod mod;
        private int index;
        public bool IsLoading => throw new NotImplementedException();
        Mod IModType.Mod { get => mod ?? GameInfos.RPG; set => mod = value; }
        int IModType.Index { get => index; set => index = value; }
    }
}
