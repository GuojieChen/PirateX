using System;

namespace PirateX.Core.Container
{
    public interface IDatabaseInitializer
    {
        void Initialize(string connectionString);
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseInitializerAttribute : Attribute
    {
        public string Name { get; }

        public DatabaseInitializerAttribute(string name)
        {
            Name = name;
        }
    }
}
