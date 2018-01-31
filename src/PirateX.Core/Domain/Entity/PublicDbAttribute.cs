using System;

namespace PirateX.Core
{
    public class PublicDbAttribute:Attribute
    {
        public string Key { get; private set; }

        public PublicDbAttribute(string key)
        {
            this.Key = key;
        }
    }
}
