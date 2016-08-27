using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX
{
    public interface IResponseConvert
    {
        byte[] Convert<T>(T t);
    }
}
