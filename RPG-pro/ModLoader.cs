using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG
{
    public class NPCLoader
    {
        public static void Load()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            Type[] types = assembly.GetTypes().Where(t => t.FullName.Contains(".NPCs.")).ToArray();
            foreach (var i in types)
            {
                var obj = Activator.CreateInstance(i);
                (obj as NPC).SetStaticDefaults();
                (obj as NPC).SetDefaults();
                NPCList.Add((int)(i.GetField("NPCType").GetValue(obj)), obj as NPC);
            }
        }
        public static Dictionary<int, NPC> NPCList = new Dictionary<int, NPC>();

    }
    public class ItemLoader
    {
        public static void Load()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            Type[] types = assembly.GetTypes().Where(t => t.FullName.Contains(".Items.")).ToArray();
            foreach (var i in types)
            {
                var obj = Activator.CreateInstance(i) as Item;
                obj.SetStaticDefaults();
                obj.SetDefaults();
                ItemList.Add((int)(i.GetField("itemType").GetValue(obj)), obj);
                if (obj.AddRecipe().Item1) Recipes.Add(obj.AddRecipe().Item2);
            }
        }
        public static Dictionary<int, Item> ItemList = new Dictionary<int, Item>();
        public static List<Recipe> Recipes = new List<Recipe>();
    }
    public class ModLoader
    {
        public static List<Assembly> assemblies = new List<Assembly>();
        public static void Load()
        {
            LoadMods();
            Assembly a = Assembly.GetEntryAssembly();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Mod))).Count() == 0)
                {
                    MessageBox.Show("加载错误: " + assembly.GetName().Name + " 此Mod需要一个Mod类");
                    continue;
                }
                Type[] types = assembly.GetTypes().Where(t => !t.IsAbstract).ToArray();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(Item))) LoadItem(type);
                    if (type.IsSubclassOf(typeof(NPC))) LoadNPC(type);
                    if (type.IsSubclassOf(typeof(Mod)))
                    {
                        var mod = Activator.CreateInstance(type) as Mod;
                        mod.OnLoad();
                        mods.Add(mod);
                    }
                }
            }
        }
        public static T GetInstance<T>() where T : class => ContentInstance<T>.Instance;
        public static int ItemType<T>() where T : Item => GetInstance<T>()?.itemType ?? 0;
        public static int NPCType<T>() where T : NPC => GetInstance<T>()?.NPCType ?? 0;
        public static void LoadMods()
        {
            string dir = Directory.GetCurrentDirectory();
            var info = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Mods");
            var files = info.GetFiles("*.dll");
            foreach (var i in files)
            {
                Assembly a = Assembly.LoadFrom(i.FullName);
                assemblies.Add(a);
            }
        }
        public static void LoadNPC(Type NPCType)
        {
            var obj = Activator.CreateInstance(NPCType) as NPC;
            obj.NPCType = NPCLoader.NPCList.Count + 1;
            obj.SetDefaults();
            Type type = typeof(ContentInstance<>).MakeGenericType(NPCType);
            PropertyInfo p = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            p.SetValue(typeof(ContentInstance<>), obj);
            NPCLoader.NPCList.Add((int)(NPCType.GetField("NPCType").GetValue(obj)), obj);
        }
        public static void LoadItem(Type ItemType)
        {
            var obj = Activator.CreateInstance(ItemType) as Item;
            obj.SetDefaults();
            obj.SetStaticDefaults();
            obj.itemType = ItemLoader.ItemList.Count + 1;
            Type type = typeof(ContentInstance<>).MakeGenericType(ItemType);
            PropertyInfo p = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            p.SetValue(typeof(ContentInstance<>), obj);
            ItemLoader.ItemList.Add((int)(ItemType.GetField("itemType").GetValue(obj)), obj);
            if (obj.AddRecipe().Item1) ItemLoader.Recipes.Add(obj.AddRecipe().Item2);
        }
        public static List<Mod> mods = new List<Mod>();
    }
    public static class ContentInstance<T> where T : class
    {
        public static T Instance { get; private set; }
    }
    public class Mod
    {
        public string DisplayName = Assembly.GetEntryAssembly().GetName().Name;
        public virtual void OnLoad()
        {

        }
        public virtual void OnEnterGame(GameForm form)
        {

        }
    }
    public class ModUI
    {
        public static void AppendModForm(Mod mod, ModForm modForm)
        {
            modForms.Add((modForm, ModLoader.mods.FindIndex(m => m.GetType() == mod.GetType())));
        }
        public static GameForm GetGameForm()
        {
            return gameForm;
        }
        public static GameForm gameForm;
        public static List<(ModForm, int)> modForms = new List<(ModForm, int)>();
    }
    public class ModForm : Form
    {
        public void AppendButton(string ButtonName, string ButtonText = "")
        {

        }
    }
}
