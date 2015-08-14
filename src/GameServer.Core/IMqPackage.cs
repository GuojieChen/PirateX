namespace GameServer.Core
{
    public interface IMqPackage
    {
        /// <summary> 封装包
        /// </summary>
        byte[] Pack();
        /// <summary> 解压包
        /// </summary>
        /// <returns></returns>
        IMqPackage UnPack();
    }
}
