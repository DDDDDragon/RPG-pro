using RPG;
using RPG_pro.Basics.Exceptions;
using RPG_pro.Basics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics
{
    internal static class ContentLookUp<T> where T : IModType
    {
        static ContentLookUp()
        {
            if (typeof(T).Assembly == typeof(ContentLookUp<>).Assembly)
            {
                TypeCaching.OnClear += delegate ()
                {
                    dict.Clear();
                    tieredDict.Clear();
                };
            }
        }
        public static void Register(T instance)
        {
            RegisterWithName(instance, instance.Name, instance.FullName);
            foreach (string legacyName in LegacyNameAttribute.GetLegacyNamesOfType(instance.GetType()))
            {
                RegisterWithName(instance, legacyName, instance.Mod.Name + "/" + legacyName);
            }
        }
        private static void RegisterWithName(T instance, string name, string fullName)
        {
            if (dict.ContainsKey(fullName))
            {
                throw new KeyAlreadyExists(fullName, nameof(dict));
            }
            dict[fullName] = instance;
            if (!tieredDict.TryGetValue(instance.Mod.Name, out Dictionary<string, T> subDictionary))
            {
                subDictionary = tieredDict[instance.Mod.Name] = new Dictionary<string, T>();
            }
            subDictionary[name] = instance;
        }
        internal static T Get(string fullName)
        {
            return dict[fullName];
        }
        internal static T Get(string modName, string contentName)
        {
            return tieredDict[modName][contentName];
        }
        internal static bool TryGetValue(string fullName, out T value)
        {
            return dict.TryGetValue(fullName, out value);
        }
        internal static bool TryGetValue(string modName, string contentName, out T value)
        {
            if (!tieredDict.TryGetValue(modName, out Dictionary<string, T> subDictionary))
            {
                value = default;
                return false;
            }
            return subDictionary.TryGetValue(contentName, out value);
        }
        private static readonly Dictionary<string, T> dict = new Dictionary<string, T>();
        private static readonly Dictionary<string, Dictionary<string, T>> tieredDict = new Dictionary<string, Dictionary<string, T>>();
    }
}
