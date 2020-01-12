﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSocket.Interfaces
{
    public interface IProtocolCoder<TProtocol>
    {
        TProtocol Decoder(byte[] data);

        byte[] EnCoder(TProtocol message);
    }
}
