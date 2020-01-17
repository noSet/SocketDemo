using System;
using CSocket.Interfaces;

namespace CSocket
{
    public class InternalChannelHandlerContext<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        /// <summary>
        /// 通道
        /// </summary>
        public DefaultChannel<TKey, TProtocol> Channel { get; set; }

        /// <summary>
        /// 未处理的字节流数组
        /// </summary>
        public byte[] UnProcessedBytes { get; set; }

        /// <summary>
        /// 已处理的字节流数组
        /// </summary>
        public byte[] ProcessedBytes { get; set; }

        /// <summary>
        /// 功能号
        /// </summary>
        public TKey Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// 管道处理状态
        /// </summary>
        public PipeStatus PipeStatus { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
