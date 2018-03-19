using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dapper;
using Dapper.Contrib.Extensions;
using GameServer.Console.SampleConfig;
using GameServer.Console.SampleDomain;
using PirateX.Core;
using StackExchange.Redis;

namespace GameServer.Console.SampleService
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
                
                Logger.Error(db.Query<Role>("select * from role").ToList());
                //Logger.Error(db.Execute("insert into role(Id,CreateAt) values(@Id,@CreateAt);",new {Id=1 ,CreateAt = DateTime.Now}));

                db.Close();
            }

            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Open();

                Logger.Error(db.Query<Role>("select * from role").ToList());
                //Logger.Error(db.Execute("insert into role(Id,CreateAt) values(@Id,@CreateAt);",new {Id=1 ,CreateAt = DateTime.Now}));

                db.Close();
            }
            
            MessageBroadcast.Send(new {Name="abc",Content="Content"},1,2);

            MessageBroadcast.SendToDistrict(new { Name = "abc", Content = "Content" }, 1,2);
        }
    }
}
