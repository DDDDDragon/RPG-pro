using RPG_pro.Basics.Exceptions;
using RPG_pro.Basics.Extensions;
using RPG_pro.Basics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    public class ModTypeLookUp<T> where T:IModType
    {
        static ModTypeLookUp()
        {
            Application.ApplicationExit += delegate { Clear(); };
        }
        public static void Register(T instance)
        {
            if(!dic.ContainsKey(instance.Mod.Name))
            {
                dic[instance.Mod.Name] = new();
            }
            dic[instance.Mod.Name][instance.GetType().FullName] = instance;
        }
        public static T Get(string modname, string fullname)
        {
            if (dic.TryGetValue(modname, out Dictionary<string, T> dic2) && dic2.TryGetValue(fullname, out T instance))
            {
                return instance;
            }
            return default;
        }
        internal static void Clear()
        {
            dic.Values.ForEach(subdic => subdic.Clear());
            dic.Clear();
        }
        static readonly Dictionary<string, Dictionary<string, T>> dic = new();
    }
}
