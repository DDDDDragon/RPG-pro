using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Exceptions
{
    public class KeyAlreadyExists:Exception
    {
        public KeyAlreadyExists(string key) : base($"{key} already exists.") { }
        public KeyAlreadyExists(string key, string keyfor) : base($"{key} already exists in {keyfor}'Keys.") { }
    }
}
