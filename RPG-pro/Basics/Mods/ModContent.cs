using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    internal class ModContent
    {
        private static readonly Dictionary<string, LocalMod> modsDirCache = new();
        private static List<string> readFailures = new();
        internal static Dictionary<string, bool> ModsEnabled = new();
        private readonly Dictionary<string, Mod> loadedMod = new();
        public IReadOnlyDictionary<string, Mod> LoadedMods => loadedMod;
        internal static void TryLoadMods(CancellationToken token)
        {
            Task.Run(() =>
            {
                LoadMods(token);
            }, CancellationToken.None);
        }
        private static void LoadMods(CancellationToken token)
        {
            var localmods = FindMods();
        }
        internal static LocalMod[] FindMods()
        {
            Directory.CreateDirectory(GameInfos.ModPath);
            List<LocalMod> localMods = new();
            HashSet<string> names = new();
            DeleteTemporaryFiles();
            string[] files = Directory.GetFiles(GameInfos.GamePath, "*.rpgmod", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                AttemptLoadMod(files[i], ref localMods, ref names);
            }
            return localMods.OrderBy(localmod => localmod.Name, StringComparer.InvariantCulture).ToArray();
        }
        private static void DeleteTemporaryFiles()
        {
            foreach (string path in GetTemporaryFiles())
            {
                Logger.Info("Cleaning up leftover temporary file " + Path.GetFileName(path));
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Logger.Error("Could not delete leftover temporary file " + Path.GetFileName(path), e);
                }
            }
        }
        private static IEnumerable<string> GetTemporaryFiles()
        {
            return Directory.GetFiles(GameInfos.ModPath, "*.rpgmod.tmp", SearchOption.TopDirectoryOnly);
        }
        private static bool AttemptLoadMod(string fileName, ref List<LocalMod> mods, ref HashSet<string> names)
        {
            DateTime lastModified = File.GetLastWriteTime(fileName);
            if (!modsDirCache.TryGetValue(fileName, out LocalMod mod) || mod.lastModified != lastModified)
            {
                try
                {
                    ModFile modFile = new ModFile(fileName, null, null);
                    using (modFile.Open())
                    {
                        mod = new LocalMod(modFile)
                        {
                            lastModified = lastModified
                        };
                    }
                }
                catch (Exception e)
                {
                    if (!readFailures.Contains(fileName))
                    {
                        Logger.Warn("Failed to read " + fileName, e);
                    }
                    else
                    {
                        readFailures.Add(fileName);
                    }
                    return false;
                }
                modsDirCache[fileName] = mod;
            }
            if (names.Add(mod.Name))
            {
                mods.Add(mod);
            }
            return true;
        }
    }
}
