namespace PirateX.Core
{
    public interface IDatabaseInitializer
    {
        void Initialize(string connectionString);
    }
}
