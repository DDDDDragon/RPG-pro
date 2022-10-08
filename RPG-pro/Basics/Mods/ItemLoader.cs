using RPG_pro.Basics.Exceptions;
using RPG_pro.Basics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Mods
{
    internal class ModLoader<T> where T : IModType
    {
        static readonly Dictionary<string, T> instances = new();
        public static void Load(T instance)
        {
            string key = instance.GetType().FullName;
            if (instances.ContainsKey(key))
            {
                throw new KeyAlreadyExists(key);
            }
            ModTypeLookUp<T>.Register(instance);
            instances[key] = instance;
            instance.Index = instances.Count;
            instance.Load();
            instance.SetStaticDefaults();
        }
    }
}
