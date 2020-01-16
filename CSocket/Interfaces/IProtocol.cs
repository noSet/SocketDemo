using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSocket.Interfaces
{
    public interface IProtocol<TKey>
    {
        TKey Code { get; set; }

        byte[] Message { get; set; }
    }
}
