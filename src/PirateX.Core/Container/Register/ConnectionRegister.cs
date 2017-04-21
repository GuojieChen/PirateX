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

            builder.Register(c => GetDbConnection(connectionString))
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

        /// <summary>
        /// 创建数据库连接对象
        /// 默认为sqlserver数据库，如果其他或者是混合情况下，需要额外处理
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected virtual IDbConnection GetDbConnection(string connectionString)
        {
            //TODO 这个不能这里写死
            return new SqlConnection(connectionString);
        }
    }


    public interface IConnectionDistrictConfig
    {
        string ConnectionString { get; set; }
    }
}
