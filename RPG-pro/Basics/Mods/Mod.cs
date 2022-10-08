using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    public class Mod
    {
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }
        public Version Version { get; internal set; }
    }
    internal class RPG : Mod
    {

    }
}