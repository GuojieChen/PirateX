namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public interface IRedisSerializer
    {
        byte[] Serilazer<T>(T obj);

        T Deserialize<T>(byte[] value);
    }
}
