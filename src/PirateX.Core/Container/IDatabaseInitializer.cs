using System;

namespace PirateX.Core.Container
{
    public interface IDatabaseInitializer
    {
        void Initialize(string connectionString);
    }
}
