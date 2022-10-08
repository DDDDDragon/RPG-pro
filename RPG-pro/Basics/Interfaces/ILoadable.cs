using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Interfaces
{
    public interface ILoadable
    {
        virtual void Load() { }
        virtual void Unload() { }
        abstract bool IsLoading { get; }
    }
}
