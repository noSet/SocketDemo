using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSocket
{
    public struct CodeMessage
    {
        public int Code { get; set; }

        public byte[] Message { get; set; }
    }
}
