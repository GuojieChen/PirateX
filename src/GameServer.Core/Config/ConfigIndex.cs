using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Config
{
    public class ConfigIndex
    {
        public string[] Names { get; private set; }

        public ConfigIndex(params string[] names)
        {
            Names = names; 
        }
    }
}
