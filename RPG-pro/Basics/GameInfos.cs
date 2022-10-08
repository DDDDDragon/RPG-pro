using RPG_pro.Basics.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics
{
    public static class GameInfos
    {
        public static Version GameVersion { get; } = Assembly.GetEntryAssembly().GetName().Version;
        public static string GamePath { get; } = Environment.CurrentDirectory;
        public static string ModPath { get; } = Path.Combine(Environment.CurrentDirectory, "Mods");
        public static string ModSourcePath { get; } = Path.Combine(Environment.CurrentDirectory, "ModSource");
        public static Mod RPG { get; } = new Mod() { Name = "RPG", DisplayName = "RPG", Version = GameVersion };
    }
}
