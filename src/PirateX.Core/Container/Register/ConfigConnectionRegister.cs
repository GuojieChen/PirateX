using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace PirateX.Core
{
    public class ConfigConnectionRegister:IDistrictConfigRegister
    {
        private static readonly IDictionary<string, IConfigReader> _configReaderDic = new Dictionary<string, IConfigReader>();

        public void Register(ContainerBuilder builder, IDistrictConfig config)
        {
            var configConnectionString = (config as IConfigConnectionDistrictConfig).ConfigConnectionString;

            //默认Config内存数据处理器
            builder.Register(c =>
                {
                    var configDbKey = configConnectionString.GetConfigDbKey();
                    if (_configReaderDic.ContainsKey(configDbKey))
                        return _configReaderDic[configDbKey];

                    var newReader = new MemoryConfigReader(
                        c.ResolveKeyed<List<Assembly>>("ConfigAssemblyList")
                        ,c.Resolve<IConfigProvider>());

                    _configReaderDic.Add(configDbKey, newReader);
                    return newReader;
                })
                .As<IConfigReader>()
                .SingleInstance();
        }

        public void SetUp(IContainer container, IDistrictConfig config)
        {
            container.Resolve<IConfigReader>()?.Load();
        }
    }


    [DistrictConfigRegister(typeof(ConfigConnectionRegister))]
    public interface IConfigConnectionDistrictConfig
    {
        string ConfigConnectionString { get; set; }
    }

}
