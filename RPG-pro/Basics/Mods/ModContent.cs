using Microsoft.VisualBasic.Logging;
using RPG;
using RPG_pro.Basics.Extensions;
using RPG_pro.Basics.Interfaces;
using RPG_pro.Basics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    internal class ModContent
    {
        static Dictionary<string, Mod> LoadedMods = new();
        private List<ModFile> FindMods()
        {
            List<string> possible = new();
            List<ModFile> mods = new();
            Utils_IO.FindFiles(possible, GameInfos.ModPath, s => s.EndsWith("rpgmod"));
            possible.ForEach(file =>
            {
                ModFile modFile = new(file);
                modFile.Open().Dispose();
                if (!modFile.Damaged)
                {
                    mods.Add(modFile);
                }
            });
            return mods;
        }
        private static void LoadMods(List<ModFile> modFiles, CancellationToken token)
        {
            ModFile Loading = null;
            try
            {
                modFiles.ForEach(modfile =>
                {
                    Loading = modfile;
                    LoadMod(modfile);
                });
            }
            catch when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception e)
            {
                Logger.Warn($"An exception was encountered during loading {Loading?.Name ?? "[Unknow]"}", e);
            }
        }
        internal static void PackegMods()
        {
            if(!Directory.Exists(GameInfos.ModSourcePath))
            {
                return;
            }
            foreach(string folder in Directory.GetDirectories(GameInfos.ModSourcePath))
            {
                string name = folder.Split("/")[^1];
                ModFile modFile = new(Path.Combine(GameInfos.ModPath, name + ".rpgmod"));
                List<string> files = new();
                Utils_IO.FindFiles(files, folder, s => true);
                files.ForEach(file =>
                {
                    modFile.AddFile(File.ReadAllBytes(file), file.Replace(folder, ""));
                });
                modFile.Save();
            }
        }
        private static void LoadMod(ModFile modFile)
        {
            using (modFile.Open())
            {
                if (modFile.RequestGameVersion is not null && modFile.RequestGameVersion != GameInfos.GameVersion)
                {
                    throw new Exception($"This module must run in the specified version:{modFile.RequestGameVersion} of the game");
                }
                Mod mod = new()
                {
                    Name = modFile.Name,
                    DisplayName = modFile.DisplayName,
                    Version = modFile.ModVersion
                };
                if (modFile.TryGetFileBytes($"{mod.Name}_Settings.rpgdata", out byte[] settingsbytes))
                {

                }
                if (modFile.TryGetFileBytes($"{mod.Name}.dll", out byte[] dllbytes))
                {
                    Assembly assembly = Assembly.Load(dllbytes);
                    assembly.GetTypes().ForEach(type =>
                    {
                        if(type is IModType)
                        {
                            LoadModType(type, mod);
                        }
                    });
                }
                else
                {
                    throw new IOException($"Critical data loss:{modFile.Name}.dll");
                }
                LoadedMods[mod.Name] = mod;
            }
        }
        private static void LoadModType(Type modtype, Mod mod)
        {
            IModType type = (IModType)Activator.CreateInstance(modtype);
            if (type.IsLoading)
            {
                type.Mod = mod;
                if(type is Item item)
                {
                    ModLoader<Item>.Load(item);
                }
                else if(type is NPC npc)
                {
                    ModLoader<NPC>.Load(npc);
                }
            }
        }
    }
}
