using System;
using System.IO;
using zlib;

namespace PirateX.Protocol
{
    public class DefaultZip:IZip
    {
        public byte[] Compress(byte[] datas)
        {
            using (var outMemoryStream = new MemoryStream())
            using (var outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
            using (Stream inMemoryStream = new MemoryStream(datas))
            {
                CopyTo(inMemoryStream, outZStream);
                outZStream.finish();
                return  outMemoryStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] datas)
        {
            using (var outMemoryStream = new MemoryStream())
            using (var outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(datas))
            {
                CopyTo(inMemoryStream, outZStream);
                outZStream.finish();
                return  outMemoryStream.ToArray();
            }
        }
        private static void CopyTo(Stream src, Stream dest)
        {
            int size = (src.CanSeek) ? Math.Min((int)(src.Length - src.Position), 0x2000) : 0x2000;
            var buffer = new byte[size];
            int n;
            do
            {
                n = src.Read(buffer, 0, buffer.Length);
                dest.Write(buffer, 0, n);
            } while (n != 0);
        }
    }
}
