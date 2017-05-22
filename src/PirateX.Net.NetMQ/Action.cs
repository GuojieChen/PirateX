using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Net.NetMQ
{
    public enum Action : byte
    {
        Ping = 0,
        Req = 1,
        Closed = 2,
        Seed = 3,    //种子交换
        Push = 4,
    }
}
