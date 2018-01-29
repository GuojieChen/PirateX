using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Container;

namespace PirateX.GMSDK.Demo
{
    public class DemoDistrictContainer:DistrictContainer<DemoDistrictContainer>
    {
        public override IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            throw new NotImplementedException();
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            throw new NotImplementedException();
        }

        protected override void BuildDistrictContainer(ContainerBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override void BuildServerContainer(ContainerBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
