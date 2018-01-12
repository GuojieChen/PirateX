using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Domain.Entity
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
