using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Interfaces
{
    public interface ILoadable : IDisposable
    {
        string Name { get; }
        string FullName { get; }
        void Load();
        void Unload();
        bool IsLoading();
    }
}
