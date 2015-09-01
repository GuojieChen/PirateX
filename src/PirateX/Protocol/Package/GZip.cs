using System.IO;
using System.IO.Compression;

namespace PirateX.Protocol.Package
{
    public class GZip:IZip
    {
        public byte[] Compress(byte[] datas)
        {
            byte[] bytes = null;

            var outMemoryStream = new MemoryStream();

            using (var outZStream = new GZipStream(outMemoryStream, CompressionMode.Compress))
            {
                outZStream.Write(datas,0,datas.Length);
                bytes = outMemoryStream.ToArray();
            }

            outMemoryStream.Close();
            outMemoryStream.Dispose();

            return bytes;
        }

        public byte[] Decompress(byte[] datas)
        {
            byte[] bytes = null; 
            var outMemoryStream = new MemoryStream();
            var inMemoryStream = new MemoryStream(datas);
            using (var outZStream = new GZipStream(inMemoryStream, CompressionMode.Decompress))
            {
                outZStream.CopyTo(outMemoryStream);
                bytes = outMemoryStream.ToArray();
            }

            inMemoryStream.Close();
            inMemoryStream.Dispose();
            outMemoryStream.Close();
            outMemoryStream.Dispose();

            return bytes;
        }
    }
}
