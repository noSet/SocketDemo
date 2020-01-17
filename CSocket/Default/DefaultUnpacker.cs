using System;
using System.Collections.Generic;
using CSocket.Interfaces;

namespace CSocket.Default
{
    /// <summary>
    /// 内置的拆包器，使用流大小判断
    /// </summary>
    public class DefaultUnpacker : IProtocolUnpacker
    {

        public IEnumerable<byte[]> Unpack(byte[] buffer, out byte[] unProcessed)
        {
            List<byte[]> handleBuffer = new List<byte[]>();

            UnpackCore(buffer, handleBuffer, out unProcessed);

            return handleBuffer;
        }

        private void UnpackCore(byte[] buffer, List<byte[]> handleBuffer, out byte[] unProcessed)
        {
            // todo BitConverter.IsLittleEndian

            // 小于4不处理
            if (buffer.Length < 4)
            {
                unProcessed = buffer;
                return;
            }

            var sizeBytes = new byte[sizeof(int)];

            Array.Copy(buffer, 0, sizeBytes, 0, 4);

            var size = BitConverter.ToInt32(sizeBytes, 0);

            // 包没读完 不处理
            if (size > buffer.Length)
            {
                unProcessed = buffer;
                return;
            }

            var message = new byte[size];

            Array.Copy(buffer, 0, message, 0, message.Length);

            handleBuffer.Add(message);

            var newMessage = new byte[buffer.Length - message.Length];

            Array.Copy(buffer, message.Length, newMessage, 0, buffer.Length - message.Length);

            buffer = newMessage;

            UnpackCore(buffer, handleBuffer, out unProcessed);
        }
    }
}
