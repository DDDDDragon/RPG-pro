using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Exceptions
{
    public class OperationRejected:Exception
    {
        public OperationRejected(string operationname) : base($"{operationname} is rejected.") { }
        public OperationRejected(string operationname,string rejecter) : base($"{operationname} rejected by {rejecter}.") { }
    }
}
