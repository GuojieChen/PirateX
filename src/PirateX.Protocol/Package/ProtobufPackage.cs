using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using ProtoBuf;

namespace PirateX.Protocol.Package
{
    /// <summary>
    /// V1的请求体
    /// protobuf的数据返回
    /// </summary>
    [ProtocolName("V2")]
    public class V2Package : AbstractProtocolPackag
    {
        private const int Version = 2;

        /// <summary>
        /// 前面的格式和V1版本的一样
        /// 不同的是将D  数据 分离开来，放到最后
        /// 
        /// [4字节版本号][4字节总体长度][1字节压缩描述][1字节加密描述][4字节信息头长度][信息头][数据体]
        /// 
        /// 其中信息头和信息体 是分别进行压缩和加密的
        /// </summary>
        /// <returns></returns>
        public override byte[] SerializeObject(ProtocolMessage message)
        {
            //数据体长度
            byte[] body = null;
            if (JsonEnable)
            {
                body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSettings));
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, message.D);
                    body = PrePack(ms.ToArray());
                }
            }
            

            message.D = null;

            var header = PrePack(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSettings)));

            var cryptoByte = new byte[1];
            cryptoByte[0] = CryptoEnable ? (byte)128 : (byte)0;

            var zipByte = new byte[1];
            zipByte[0] = ZipEnable ? (byte)128 : (byte)0;

            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(Version),0,4);//版本号 此版本为2
                stream.Write(BitConverter.GetBytes(header.Length + body.Length + 6 +4), 0, 4);//数据总体长度
                stream.Write(zipByte, 0, 1);//压缩
                stream.Write(cryptoByte, 0, 1);//加密
                stream.Write(BitConverter.GetBytes(header.Length), 0, 4);//信息头长度
                stream.Write(header, 0, header.Length);//信息头内容
                stream.Write(body, 0,body.Length);//数据体内容 

                return stream.ToArray();
            }
        }

        /// <summary>
        /// 数据装包前的工作
        /// 压缩和加密控制
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private byte[] PrePack(byte[] datas)
        {
            var compress = ZipEnable ? Zip.Compress(datas) : datas;

            var d = CryptoEnable ? Crypto.Encode(compress, ServerKeys[0]) : compress;

            return d;
        }


        /// <summary>
        /// JsonPackage的方式
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public override IGameRequestInfo DeserializeObject(byte[] datas)
        {
            var body = Encoding.UTF8.GetString(base.Unpack(datas));

            var jObject = JObject.Parse(body);

            return new JsonRequestInfo(jObject["C"]?.ToString(), jObject["D"], Convert.ToBoolean(jObject["R"]), Convert.ToInt32(jObject["O"]));
        }


        /// <summary>
        /// JSON 序列化配置
        /// </summary>
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                var timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                };

                settings.Converters.Add(timeConverter);
                settings.Converters.Add(new DoubleConverter());

                return settings;
            }
        }
    }
}
