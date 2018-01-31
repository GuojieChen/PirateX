using System.ComponentModel.DataAnnotations;
using System.IO;
using ProtoBuf;

namespace PirateX.Protocol.ResponseConvert
{
    [DisplayColumn("protobuf")]
    public class ProtoResponseConvert: IResponseConvert
    {
        public byte[] SerializeObject<T>(T message)
        {
            if (Equals(message,default(T)))
                return null; 

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }

        public T DeserializeObject<T>(byte[] datas)
        {
            if (datas == null)
                return default(T);

            using (var ms = new MemoryStream(datas))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
