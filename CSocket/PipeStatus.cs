namespace CSocket
{
    /// <summary>
    /// 管道状态
    /// </summary>
    public enum PipeStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,

        /// <summary>
        /// 拆包失败
        /// </summary>
        UnPackError,

        /// <summary>
        /// 获取消息字节流失败
        /// </summary>
        DecoderError,

        /// <summary>
        /// 反序列失败
        /// </summary>
        DeserializeError,
    }
}