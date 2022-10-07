using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Network
{
    /// <summary>
    /// 数据包.
    /// </summary>
    public struct DataPackage
    {
        /// <summary>
        /// 数据包标识符.
        /// </summary>
        public string Identifier;

        /// <summary>
        /// 包数据.
        /// </summary>
        public byte[] Datas;

    }
}
