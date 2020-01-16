using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSocket.Interfaces
{
    /// <summary>
    /// 拆包器
    /// </summary>
    public interface IProtocolUnpacker
    {
        /// <summary>
        /// 字节流拆分成多个完整的包
        /// </summary>
        /// <param name="buffer">已接收的字节流</param>
        /// <param name="unProcessed">未处理的字节流（不够一个包）</param>
        /// <returns>完整的包列表</returns>
        IEnumerable<byte[]> Unpack(byte[] buffer, out byte[] unProcessed);
    }
}
