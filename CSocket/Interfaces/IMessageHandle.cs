using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSocket.Interfaces
{
    public interface IMessageHandle<TProtocol>
    {
        void Handle(TProtocol message);
    }
}
