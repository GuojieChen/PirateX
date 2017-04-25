using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Core.Container.Register
{
    public class ConnectionRegister:IDistrictConfigRegister
    {
        public void Register(ContainerBuilder builder, IDistrictConfig config)
        {
            var connectionString = (config as IConnectionDistrictConfig).ConnectionString;

            builder.Register(c => c.Resolve<IDbConnection>(new NamedParameter("ConnectionString", connectionString)))
                .As<IDbConnection>()
                .InstancePerDependency();
        }

        public void SetUp(IContainer container,IDistrictConfig config)
        {
            var connectionString = (config as IConnectionDistrictConfig).ConnectionString;

            if (container.IsRegistered<IDatabaseInitializer>())
            {
                //判断是否有更新 ？
                //更新数据库
                container.Resolve<IDatabaseInitializer>().Initialize(connectionString);
            }
        }
    }


    public interface IConnectionDistrictConfig
    {
        string ConnectionString { get; set; }
    }
}
