namespace PirateX.Protocol.Package
{
    public interface IResponseConvert
    {
        byte[] SerializeObject<T>(T t);

        T DeserializeObject<T>(byte[] datas);
    }
}
