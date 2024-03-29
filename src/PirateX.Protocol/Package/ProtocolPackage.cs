﻿using System;
using System.IO;
using System.Net;

namespace PirateX.Protocol
{
    public class ProtocolPackage : IProtocolPackage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public int Rid { get; set; }

        public EndPoint RemoteEndPoint { get; set; }

        public ProtocolPackage(IZip zip)
        {
            ZipEnable = false;
            Zip = zip;
        }

        public ProtocolPackage() : this(new DefaultZip())
        {
        }

        public IZip Zip { get; private set; }
        public bool ZipEnable { get; set; } = true;
        public byte CryptoByte { get; set; }

        public byte[] PackKeys { get; set; }
        public byte[] UnPackKeys { get; set; }

        public int LastNo { get; set; } = -1;

        public byte[] PackPacketToBytes(IPirateXPackage requestPackage)
        {
            if (requestPackage == null)
                return null;

            var headerbytes = requestPackage.HeaderBytes;
            var contentbytes = requestPackage.ContentBytes;
            //信息头压缩
            headerbytes = ZipEnable ? Zip.Compress(headerbytes) : headerbytes;
            //数据体压缩
            contentbytes = ZipEnable ? Zip.Compress(contentbytes) : contentbytes;

            //8位,  每一位是一个加密标记位，1表示启用
            for (byte i = 0; i < 8; i++)
            {
                if (CryptoByte.GetBit(i))
                {
                    var crypto = CryptoFactory.GetCrypto(i);

                    if (crypto != null)
                    {
                        //信息头加密
                        headerbytes = crypto.Encode(headerbytes, PackKeys);
                        //数据体加密
                        if(contentbytes == null)
                            contentbytes = new byte[0];
                        else 
                            contentbytes = crypto.Encode(contentbytes, PackKeys);
                    }
                }
            }

            var zipByte = new byte[1];
            zipByte[0] = ZipEnable ? (byte)Math.Pow(2, 7) : (byte)0;

            var headerLen = headerbytes.Length;
            var contentLen = contentbytes?.Length ?? 0;

            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(4 + 1 + 1 + 4 + headerLen + contentLen), 0, 4);
                stream.Write(zipByte, 0, 1);
                stream.Write(new[] { CryptoByte }, 0, 1);
                stream.Write(BitConverter.GetBytes(headerLen), 0, 4);
                stream.Write(headerbytes, 0, headerLen);
                if(contentbytes!=null )
                    stream.Write(contentbytes, 0, contentLen);
                return stream.ToArray();
            }
        }

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

            //8位,  每一位是一个加密标记位，1表示启用
            for (byte i = 0; i < 8; i++)
            {
                if (cryptoBit[0].GetBit(i))
                {
                    var crypto = CryptoFactory.GetCrypto(i);
                    if (crypto != null)
                    {
                        headerBytes = crypto.Decode(headerBytes, UnPackKeys);
                        contentBytes = crypto.Decode(contentBytes, UnPackKeys);
                    }
                }
            }

            var zipenable = zipBit[0].GetBit(7);

            if (zipenable)
            {
                headerBytes = Zip.Decompress(headerBytes);
                contentBytes = Zip.Decompress(contentBytes);
            }

            return new PirateXResponsePackage()
            {
                HeaderBytes = headerBytes,
                ContentBytes = contentBytes
            };
        }

        public virtual void Send(byte[] body)
        {
            
        }

        public virtual void Close()
        {
            
        }

        //private static int GetbitValue(byte input, int index)
        //{
        //    return (input & ((uint)1 << index)) > 0 ? 1 : 0;
        //}
    }
}