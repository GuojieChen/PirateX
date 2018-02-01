using Autofac;

namespace PirateX.Core
{
    public class ConnectionRegister:IDistrictConfigRegister
    {
        public void Register(ContainerBuilder builder, IDistrictConfig config)
        {
            var connectionString = ((IConnectionDistrictConfig)config).ConnectionString;
            builder.Register<string>(c=>connectionString)
                .Named<string>("ConnectionString");
        }

        public void SetUp(IContainer container,IDistrictConfig config)
        {
            var connectionString = ((IConnectionDistrictConfig)config).ConnectionString;

            if (container.IsRegistered<IDatabaseInitializer>())
            {
                //判断是否有更新 ？
                //更新数据库
                container.Resolve<IDatabaseInitializer>().Initialize(connectionString);
            }
        }
    }

    [DistrictConfigRegister(typeof(ConnectionRegister))]
    public interface IConnectionDistrictConfig
    {
        string ConnectionString { get; set; }
    }
}
