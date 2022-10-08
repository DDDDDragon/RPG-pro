using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Utils
{
    public class Utils_Normal
    {
        public class DisposeWrapper : IDisposable
        {
            public DisposeWrapper(Action action)
            {
                dispose = action;
            }
            Action dispose;
            public void Dispose()
            {
                dispose?.Invoke();
            }
        }
    }
}
