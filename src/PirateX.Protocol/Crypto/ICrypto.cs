namespace PirateX.Protocol
{
    public interface ICrypto
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Encode(byte[] data,byte[] serverKey);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        byte[] Decode(byte[] datas,byte[] clientKey); 
    }
}
