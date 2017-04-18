using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace PirateX.Protocol.Package
{
    [DisplayColumn("protobuf")]
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
            using (var ms = new MemoryStream(datas))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
