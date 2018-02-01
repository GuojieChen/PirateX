namespace PirateX.Core
{
    public interface IRedisSerializer
    {
        byte[] Serilazer<T>(T obj);

        T Deserialize<T>(byte[] value);
    }
}
