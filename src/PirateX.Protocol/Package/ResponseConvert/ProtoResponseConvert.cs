using System.ComponentModel.DataAnnotations;
using System.IO;
using ProtoBuf;

namespace PirateX.Protocol.Package.ResponseConvert
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
