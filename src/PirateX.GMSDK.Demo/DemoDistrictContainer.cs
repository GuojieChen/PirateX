using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;

namespace PirateX.GMSDK.Demo
{
    public class DemoDistrictContainer:DistrictContainer<DemoDistrictContainer>
    {
        private IEnumerable<IDistrictConfig> _list = new DistrictConfig[]
        {
            new DistrictConfig{ Id = 1,Name="S1" },
            new DistrictConfig{ Id = 2,Name="S2" },
            new DistrictConfig{ Id = 3,Name="S3" },
            new DistrictConfig{ Id = 4,Name="S4" },
            new DistrictConfig{ Id = 1,Name="S5" },
            new DistrictConfig{ Id = 2,Name="S6" },
            new DistrictConfig{ Id = 3,Name="S7" },
            new DistrictConfig{ Id = 4,Name="S8" },
            new DistrictConfig{ Id = 1,Name="S9" },
            new DistrictConfig{ Id = 2,Name="S10" },
            new DistrictConfig{ Id = 3,Name="S11" },
            new DistrictConfig{ Id = 4,Name="S12" },
            new DistrictConfig{ Id = 1,Name="S13" },
            new DistrictConfig{ Id = 2,Name="S14" },
            new DistrictConfig{ Id = 3,Name="S15" },
            new DistrictConfig{ Id = 4,Name="S16" },
        };

        public override IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            return _list;
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            return _list.FirstOrDefault(item => item.Id == id);
        }

        protected override void BuildDistrictContainer(ContainerBuilder builder)
        {
        }

        protected override void BuildServerContainer(ContainerBuilder builder)
        {
        }
    }

    public class DistrictConfig : IDistrictConfig
    {
        public int Id { get; set; }
        public string SecretKey { get; set; }
        public int TargetId { get; set; }

        public string Name { get; set; }
    }
}
