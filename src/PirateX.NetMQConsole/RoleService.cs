using System;
using System.Data;
using Autofac;
using PirateX.Core;
using PirateX.Core.Broadcas;
using PirateX.Core.Config;
using StackExchange.Redis;

namespace PirateX.NetMQConsole
{
    public class RoleService : ServiceBase
    {
        public IMessageBroadcast MessageBroadcast { get; set; }

        public void ShowLog()
        {

            Resolver.Resolve<IDatabase>().StringSet("test", "aaaaaa");
            Resolver.Resolve<IDatabase>().StringSet("test2", "bbbbb");

            Resolver.Resolve<IConfigReader>().SingleByIndexes<PetConfig>(new
            {
                AwakeCost = 1,
                ElementType=2
            }); 

            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Open();

                //Logger.Error(db.Execute("insert into role(Id,CreateAt) values(@Id,@CreateAt);",new {Id=1 ,CreateAt = DateTime.Now}));

                db.Close();
            }

            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Open();

                //Logger.Error(db.Execute("insert into role(Id,CreateAt) values(@Id,@CreateAt);",new {Id=1 ,CreateAt = DateTime.Now}));

                db.Close();
            }
            
            MessageBroadcast.Send(new {Name="abc",Content="Content"},1,2);

            MessageBroadcast.SendToDistrict(new { Name = "abc", Content = "Content" }, 1,2);

            using (var uow = CreateUnitOfWork())
            {
                //uow.Repository<>()

                uow.Repository<RoleRepository>().Insert(new Role() {Id = 1,Name = "Test", });

                uow.Repository<RoleRepository>().GetById(1);


                using (var now2 = CreateUnitOfWork(""))
                {
                }

                uow.Commit();
            }
        }
    }
}
