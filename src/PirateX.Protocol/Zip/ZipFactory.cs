namespace PirateX.Protocol.Zip
{
    public class ZipFactory
    {
        private static readonly IZip Default = new DefaultZip();
        public static IZip GetZip()
        {
            return Default; 
        }
    }
}
