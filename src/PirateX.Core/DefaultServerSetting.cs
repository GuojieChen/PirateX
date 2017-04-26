using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Container;
using PirateX.Core.Container.ServerSettingRegister;

namespace PirateX.Core
{
    public class DefaultServerSetting : IServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
