using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Config;
using PirateX.Core.Utils;

namespace PirateX.Core.Container.Register
{
    public class ConfigConnectionRegister:IDistrictConfigRegister
    {
        private readonly IDictionary<string, IConfigReader> _configReaderDic = new Dictionary<string, IConfigReader>();

        public void Register(ContainerBuilder builder, IDistrictConfig config)
        {
            var configConnectionString = (config as IConfigConnectionDistrictConfig).ConfigConnectionString;

            //默认Config内存数据处理器
            builder.Register(c =>
                {
                    var configDbKey = configConnectionString.GetConfigDbKey();
                    if (_configReaderDic.ContainsKey(configDbKey))
                        return _configReaderDic[configDbKey];

                    var newReader = new MemoryConfigReader(c.Resolve<List<Assembly>>(), () => c.Resolve<IDbConnection>(new NamedParameter("ConnectionString", configConnectionString)));
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
