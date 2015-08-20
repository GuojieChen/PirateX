using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Package
{
    public class NoneZip:IZip
    {
        public byte[] Compress(byte[] datas)
        {
            return datas;
        }

        public byte[] Decompress(byte[] datas)
        {
            return datas; 
        }
    }
}
