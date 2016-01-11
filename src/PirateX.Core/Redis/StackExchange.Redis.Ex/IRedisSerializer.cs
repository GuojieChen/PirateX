namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public interface IRedisSerializer
    {
        string Serilazer<T>(T obj);

        T Deserialize<T>(string value);
    }
}
