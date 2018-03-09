using System;
using System.Collections.Generic;
using System.Data;
using Autofac;
using Dapper;
using Dapper.Contrib.Extensions;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class MidLetterRepository<TLetter>:RepositoryBase
        where TLetter :class,ILetter
    {
        public virtual int Insert(ILetter letter)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return (int)db.Insert(letter);
            }
        }

        public virtual void Insert(IEnumerable<ILetter> letters)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Insert(letters);
            }
        }

        public virtual int Delete(int id)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Execute($"delete from `{typeof(TLetter).Name}` where Id = {id}");
            }

            //TODO 这里可以根据情况做下归档
        }

        public virtual void SetRead(int id)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Execute($"update `{typeof(TLetter).Name}` set IsRead = @IsRead where Id=@Id",new { IsRead  =true,Id = id});
            }
        }

        public virtual IEnumerable<TLetter> GetList(int rid, int page, int size = 50) 
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Query<TLetter>($"select * from `{typeof(TLetter).Name}` where Rid = {rid} limit {(page - 1)*size},{page*size}");
            }
        }


    }
}
