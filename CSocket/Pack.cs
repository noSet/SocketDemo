using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSocket
{
    /// <summary>
    /// 包
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    public abstract class Pack
    {
        /// <summary>
        /// 功能号
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public virtual byte[] Message { get; }

        ///// <summary>
        ///// 编码
        ///// </summary>
        //public abstract byte[] Encoder(object message);

        ///// <summary>
        ///// 解码
        ///// </summary>
        //public abstract void Decoder(byte[] buffer);
    }
}
