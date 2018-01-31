namespace PirateX.Protocol.ResponseConvert
{
    public interface IResponseConvert
    {
        byte[] SerializeObject<T>(T t);

        T DeserializeObject<T>(byte[] datas);
    }
}
