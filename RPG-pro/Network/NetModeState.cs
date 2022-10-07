using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Network
{
    public enum NetModeState
    {
        /// <summary>
        /// 进行.
        /// </summary>
        Conduct,

        /// <summary>
        /// 等待并挂起.
        /// </summary>
        Await,

        /// <summary>
        /// 直接中止.
        /// </summary>
        Stop,

        /// <summary>
        /// 完成.
        /// </summary>
        Over
    }
}
