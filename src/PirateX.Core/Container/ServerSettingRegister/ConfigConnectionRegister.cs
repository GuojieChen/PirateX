using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace PirateX.Core
{
    public class ConfigConnectionRegister: IServerSettingRegister
    {
        public void Register(ContainerBuilder builder, IServerSetting setting)
        {
            //默认Config内存数据处理器
            builder.Register(c =>
                {
                    var newReader = new MemoryConfigReader(
                        c.ResolveKeyed<List<Assembly>>("ConfigAssemblyList")
                        ,c.Resolve<IConfigProvider>());

                    return newReader;
                })
                .As<IConfigReader>()
                .SingleInstance();
        }

        public void SetUp(IContainer container, IServerSetting setting)
        {
            container.Resolve<IConfigReader>()?.Load();
        }
    }


    [ServerSettingRegister(typeof(ConfigConnectionRegister))]
    public interface IConfigConnectionDistrictConfig
    {
        string ConfigConnectionString { get; set; }
    }

}
