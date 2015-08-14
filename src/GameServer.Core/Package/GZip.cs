using System.IO;
using System.IO.Compression;

namespace GameServer.Core.Package
{
    public class GZip:IZip
    {
        public byte[] Compress(byte[] datas)
        {
            using (var outMemoryStream = new MemoryStream())

            using (var outZStream = new GZipStream(outMemoryStream, CompressionLevel.Fastest,true))
            using (Stream inMemoryStream = new MemoryStream(datas))
            {
                inMemoryStream.CopyTo(outZStream);
                return outMemoryStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] datas)
        {
            using (var outMemoryStream = new MemoryStream())
            using (var outZStream = new GZipStream(outMemoryStream, CompressionMode.Decompress))
            using (Stream inMemoryStream = new MemoryStream(datas))
            {
                inMemoryStream.CopyTo(outZStream);
                return  outMemoryStream.ToArray();
            }
        }
    }
}
