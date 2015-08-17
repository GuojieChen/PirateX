using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Localization
{
    public class LocalizationString
    {
        private ResourceManager rm = null;

        public LocalizationString(ResourceManager resourceManager)
        {
            rm = resourceManager; 
        }

    }
}
