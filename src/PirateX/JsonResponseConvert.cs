using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PirateX
{
    public class JsonResponseConvert : IResponseConvert
    {
        public byte[] Convert<T>(T t)
        {
            var jsonStr = JsonConvert.SerializeObject(t);
            return Encoding.UTF8.GetBytes(jsonStr);
        }
    }
}
