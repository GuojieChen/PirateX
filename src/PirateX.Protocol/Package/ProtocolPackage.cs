using System;
using System.Collections.Generic;
using System.IO;
using PirateX.Protocol.Crypto;
using PirateX.Protocol.Zip;

namespace PirateX.Protocol.Package
{
    public class ProtocolPackage : IProtocolPackage
    {
        public string SessionID { get; set; } = Guid.NewGuid().ToString("N");

        public ProtocolPackage(IZip zip, ICrypto crypto)
        {
            ZipEnable = true;
            Zip = zip;
            Crypto = crypto;
        }

        public ProtocolPackage() : this(new DefaultZip(), new XXTea())
        {
        }

        public IZip Zip { get; private set; }
        public ICrypto Crypto { get; set; }
        //public IResponseConvert ResponseConvert { get; set; }

        public bool ZipEnable { get; set; }
        public bool CryptoEnable { get; set; }
        public bool JsonEnable { get; set; }
        public byte[] PackKeys { get; set; }
        public byte[] UnPackKeys { get; set; }

        /*
        public IPirateXRequestPackage UnPackToRequestPackage(byte[] datas)
        {
            byte[] headerBytes = null;
            byte[] contentBytes = null;

            var zipBit = new byte[1];
            var cryptoBit = new byte[1];

            using (var stream = new MemoryStream(datas))
            {
                var lenBytes = new byte[4];
                var headerLenBytes = new byte[4];

                stream.Read(lenBytes, 0, 4);//数据整体长度
                stream.Read(zipBit, 0, 1); //压缩标记位
                stream.Read(cryptoBit, 0, 1);//加密标记位
                stream.Read(headerLenBytes, 0, 4);//信息头长度

                var len = BitConverter.ToInt32(lenBytes, 0);
                var headerLen = BitConverter.ToInt32(headerLenBytes, 0);

                headerBytes = new byte[headerLen];
                contentBytes = new byte[len - 4 - 1 - 1 - 4 - headerLen];

                stream.Read(headerBytes, 0, headerLen);
                stream.Read(contentBytes, 0, len - 4 - 1 - 1 - 4 - headerLen);
            }
            for (var i = 0; i < 8; i++)
            {
                var v = GetbitValue(cryptoBit[0], i);
                if (v == 1)
                {
                    headerBytes = Crypto.Decode(headerBytes, UnPackKeys);
                    contentBytes = Crypto.Decode(contentBytes, UnPackKeys);
                }
            }

            var zipenable = GetbitValue(zipBit[0], 7) == 1;

            if (zipenable)
            {
                headerBytes = Zip.Decompress(headerBytes);
                contentBytes = Zip.Decompress(contentBytes);
            }

            //return new PirateXRequestInfo(HttpUtility.ParseQueryString(Encoding.UTF8.GetString(headerBytes))
            //    , HttpUtility.ParseQueryString(Encoding.UTF8.GetString(contentBytes)));

            return new PirateXRequestPackage()
            {
                HeaderBytes = headerBytes,
                ContentBytes = contentBytes
            };
        }
        */
        public byte[] PackPacketToBytes(IPirateXPackage requestPackage)
        {
            if (requestPackage == null)
                return null;

            //var headerNc = requestinfo.Headers;
            //var headerbytes = Encoding.UTF8.GetBytes($"{String.Join("&", headerNc.AllKeys.Select(a => a + "=" + headerNc[a]))}");
            //var contentbytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestinfo.QueryString.AllKeys.Select(a => a + "=" + requestinfo.QueryString[a]))}");

            var headerbytes = requestPackage.HeaderBytes;
            var contentbytes = requestPackage.ContentBytes;

            //信息头压缩
            headerbytes = ZipEnable ? Zip.Compress(headerbytes) : headerbytes;
            //信息头加密
            headerbytes = CryptoEnable ? Crypto.Encode(headerbytes, PackKeys) : headerbytes;

            //数据体压缩
            contentbytes = ZipEnable ? Zip.Compress(contentbytes) : contentbytes;
            //数据体加密
            contentbytes = CryptoEnable ? Crypto.Encode(contentbytes, PackKeys) : contentbytes;

            var cryptoByte = new byte[1];
            cryptoByte[0] = CryptoEnable ? (byte)128 : (byte)0;

            var zipByte = new byte[1];
            zipByte[0] = ZipEnable ? (byte)Math.Pow(2, 7) : (byte)0;

            var headerLen = headerbytes.Length;
            var contentLen = contentbytes.Length;

            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes( 4 + 1 + 1 + 4 + headerLen + contentLen ), 0, 4);
                stream.Write(zipByte, 0, 1);
                stream.Write(cryptoByte, 0, 1);
                stream.Write(BitConverter.GetBytes(headerLen), 0, 4);
                stream.Write(headerbytes, 0, headerLen);
                stream.Write(contentbytes, 0, contentLen);
                return stream.ToArray();
            }
        }
        /*
        public byte[] PackResponsePackageToBytes(IPirateXResponsePackage respolnsePackage)
        {
            //var headers = responseInfo.Headers;
            //var headerbytes = Encoding.UTF8.GetBytes($"{String.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a]))}");
            //var contentbytes = (data == null) ? new byte[0] : ResponseConvert.SerializeObject(data);

            var headerbytes = respolnsePackage.HeaderBytes;
            var contentbytes = respolnsePackage.ContentBytes;

            //信息头压缩
            headerbytes = ZipEnable ? Zip.Compress(headerbytes) : headerbytes;
            //信息头加密
            headerbytes = CryptoEnable ? Crypto.Encode(headerbytes, PackKeys) : headerbytes;

            //数据体压缩
            contentbytes = ZipEnable ? Zip.Compress(contentbytes) : contentbytes;
            //数据体加密
            contentbytes = CryptoEnable ? Crypto.Encode(contentbytes, PackKeys) : contentbytes;


            var cryptoByte = new byte[1];
            cryptoByte[0] = CryptoEnable ? (byte)128 : (byte)0;

            var zipByte = new byte[1];
            zipByte[0] = ZipEnable ? (byte)Math.Pow(2, 7) : (byte)0;

            var headerLen = headerbytes.Length;
            var contentLen = contentbytes.Length;

            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(headerLen + contentLen + 4 + 1 + 1 + 4), 0, 4);
                stream.Write(zipByte, 0, 1);
                stream.Write(cryptoByte, 0, 1);
                stream.Write(BitConverter.GetBytes(headerLen), 0, 4);
                stream.Write(headerbytes, 0, headerLen);
                stream.Write(contentbytes, 0, contentLen);
                return stream.ToArray();
            }
        }
        */
        public IPirateXPackage UnPackToPacket(byte[] datas)
        {
            byte[] headerBytes = null;
            byte[] contentBytes = null;

            var zipBit = new byte[1];
            var cryptoBit = new byte[1];

            using (var stream = new MemoryStream(datas))
            {
                var lenBytes = new byte[4];
                var headerLenBytes = new byte[4];

                stream.Read(lenBytes, 0, 4);//数据整体长度
                stream.Read(zipBit, 0, 1); //压缩标记位
                stream.Read(cryptoBit, 0, 1);//加密标记位
                stream.Read(headerLenBytes, 0, 4);//信息头长度

                var len = BitConverter.ToInt32(lenBytes, 0);
                var headerLen = BitConverter.ToInt32(headerLenBytes, 0);

                headerBytes = new byte[headerLen];
                contentBytes = new byte[len - 4 - 1 - 1 - 4 - headerLen];

                stream.Read(headerBytes, 0, headerLen);
                stream.Read(contentBytes, 0, len - 4 - 1 - 1 - 4 - headerLen);
            }
            for (var i = 0; i < 8; i++)
            {
                var v = GetbitValue(cryptoBit[0], i);
                if (v == 1)
                {
                    headerBytes = Crypto.Decode(headerBytes, UnPackKeys);
                    contentBytes = Crypto.Decode(contentBytes, UnPackKeys);
                }
            }

            var zipenable = GetbitValue(zipBit[0], 7) == 1;

            if (zipenable)
            {
                headerBytes = Zip.Decompress(headerBytes);
                contentBytes = Zip.Decompress(contentBytes);
            }
            
            //return new PirateXResponse(contentBytes)
            //{
            //    Headers = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(headerBytes)),
            //};

            return new PirateXResponsePackage()
            {
                HeaderBytes = headerBytes,
                ContentBytes = contentBytes
            };
        }

        private static int GetbitValue(byte input, int index)
        {
            return (input & ((uint)1 << index)) > 0 ? 1 : 0;
        }
    }
}