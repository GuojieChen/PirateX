namespace PirateX.Protocol.Zip
{
    public interface IZip
    {
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="datas">元数据字节数组</param>
        /// <returns></returns>
        byte[] Compress(byte[] datas);
        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="datas">元数据字节数组</param>
        /// <returns></returns>
        byte[] Decompress(byte[] datas);
    }
}
