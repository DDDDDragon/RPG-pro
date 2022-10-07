using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Network
{
    public enum NetType
    {
        /// <summary>
        /// 多人主机.
        /// </summary>
        NetHost,
        /// <summary>
        /// 多人客户端.
        /// </summary>
        NetClient,
        /// <summary>
        /// 单机.
        /// </summary>
        Alone
    }
}
