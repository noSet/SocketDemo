using CSocket.Interfaces;
using System;
using System.Collections.Generic;

namespace CSocket
{
    /// <summary>
    /// 内置的拆包器，使用流大小判断
    /// </summary>
    public class DefaultUnpacker : IUnpacker
    {
        private byte[] _unhandledBuffer = new byte[0];

        public IEnumerable<byte[]> Unpack(byte[] buffer)
        {
            Array.Resize(ref _unhandledBuffer, _unhandledBuffer.Length + buffer.Length);
            Array.Copy(buffer, 0, _unhandledBuffer, _unhandledBuffer.Length - buffer.Length, buffer.Length);

            List<byte[]> handleBuffer = new List<byte[]>();

            UnpackCore(handleBuffer);

            return handleBuffer;
        }

        private void UnpackCore(List<byte[]> handleBuffer)
        {
            // 小于4不处理
            if (_unhandledBuffer.Length < 4)
            {
                return;
            }

            var size = new byte[4];

            Array.Copy(_unhandledBuffer, 0, size, 0, 4);

            // 包没读完 不处理
            if (BitConverter.ToInt32(size, 0) > _unhandledBuffer.Length)
            {
                return;
            }

            var message = new byte[BitConverter.ToInt32(size, 0)];

            Array.Copy(_unhandledBuffer, 0, message, 0, message.Length);

            handleBuffer.Add(message);

            var newMessage = new byte[_unhandledBuffer.Length - message.Length];

            Array.Copy(_unhandledBuffer, message.Length, newMessage, 0, _unhandledBuffer.Length - message.Length);

            _unhandledBuffer = newMessage;

            UnpackCore(handleBuffer);
        }
    }
}
