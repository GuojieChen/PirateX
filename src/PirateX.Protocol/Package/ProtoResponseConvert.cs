using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Protocol.Package
{
    public class ProtoResponseConvert: IResponseConvert
    {
        public byte[] SerializeObject<T>(T message)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }

        public T DeserializeObject<T>(byte[] datas)
        {
            throw new NotImplementedException();
        }
    }
}
