using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Extensions
{
    public static class Normal
    {
        public static void ForEach<T>(this IEnumerable<T> values,Action<T> action)
        {
            if(action is not null)
            {
                var Enumerator = values.GetEnumerator();
                while(Enumerator.MoveNext())
                {
                    action(Enumerator.Current);
                }
            }
        }
    }
}
