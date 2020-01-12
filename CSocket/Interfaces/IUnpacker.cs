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
    public interface IUnpacker
    {
        /// <summary>
        /// 将一个字节流拆出多个完整的消息
        /// </summary>
        /// <param name="buffer">字节流</param>
        /// <returns>完整的消息</returns>
        IEnumerable<byte[]> Unpack(byte[] buffer);
    }
}
