using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using PirateX.Core.Container;
using PirateX.Core.Container.Register;
using PirateX.Core.Container.ServerSettingRegister;
using ServiceStack.OrmLite;

namespace PirateX.Core.UnitTest
{
    public class DistrictContainerTestContainer : DistrictContainer<DistrictContainerTestContainer>
    {
        public static string ConfigConnectionString = "Data Source=:memory:;Version=3;New=True;"; 

        private DistrictContainerTestDistrictConfig _districtConfig = new DistrictContainerTestDistrictConfig()
        {
            Id = 110,
            SecretKey = "",
            ConfigConnectionString = ConfigConnectionString,
            ConnectionString = "Data Source=:memory:;Version=3;New=True;",

        };


        public override IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            return new IDistrictConfig[] { _districtConfig };
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            return _districtConfig;
        }

        protected override void BuildServerContainer(ContainerBuilder builder)
        {
            builder.Register<IDbConnection>((c, p) => new SQLiteConnection(p.Named<string>("ConnectionString")))
                .As<IDbConnection>()
                .InstancePerDependency();
        }

        protected override void BuildDistrictContainer(ContainerBuilder builder)
        {
            builder.Register<IDbConnection>((c, p) => new SQLiteConnection(p.Named<string>("ConnectionString")))
                .As<IDbConnection>()
                .InstancePerDependency();
        }
    }

    public class DistrictContainerTestDistrictConfig : IDistrictConfig
        , IConnectionDistrictConfig
        , IConfigConnectionDistrictConfig
    {
        public int Id { get; set; }
        public string SecretKey { get; set; }
        public string ConnectionString { get; set; }
        public string ConfigConnectionString { get; set; }
    }


    public class DistrictContainerTestServerSetting : IServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


    [TestFixture]
    public class DistrictContainerTest
    {
        private DistrictContainerTestContainer container;

        [SetUp]
        public void SetUp()
        {
            container = new DistrictContainerTestContainer();
            var builder = new ContainerBuilder();

            container.InitContainers(builder);
        }

        [Test]
        public void ResolveConnection()
        {
            var dbConn = container.ServerIoc.Resolve<IDbConnection>(
                new NamedParameter("ConnectionString", DistrictContainerTestContainer.ConfigConnectionString)
                );

            dbConn.Open();

            dbConn.Close();
        }
    }
}
