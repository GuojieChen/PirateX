﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dapper;
using GameServer.Console.SampleDomain;
using PirateX.Core;
using PirateX.Core.Service;
using StackExchange.Redis;

namespace GameServer.Console.SampleService
{
    public class RoleService : GameService
    {
        public void ShowLog()
        {

            Resolver.Resolve<IDatabase>().StringSet("test", "aaaaaa");

            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Open();

                Logger.Error(db.Query<Role>("select * from role").ToList());
                //Logger.Error(db.Execute("insert into role(Id,CreateAt) values(@Id,@CreateAt);",new {Id=1 ,CreateAt = DateTime.Now}));

                db.Close();
            } 
        }
    }
}
