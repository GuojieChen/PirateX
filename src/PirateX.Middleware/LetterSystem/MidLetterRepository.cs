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

        public virtual void SetRead(IEnumerable<int> ids)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Execute($"update `{typeof(TLetter).Name}` set IsRead = @IsRead where Id in @Ids",new { IsRead  = true,Ids = new SqlinList<int>(ids) });
            }
        }

        public virtual IEnumerable<TLetter> GetList(int rid, int page, int size = 50) 
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Query<TLetter>($"select * from `{typeof(TLetter).Name}` where Rid = {rid} limit {(page - 1)*size},{page*size}");
            }
        }

        public virtual int GetCount(int rid)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.ExecuteScalar<int>($"select count(id) from `{typeof(TLetter).Name}` where rid = {rid};");
            }
        }

        public IEnumerable<TLetter> GetAll(int rid)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Query<TLetter>($"select * from `{typeof(TLetter).Name}` where rid = {rid};");
            }
        }

        public TLetter GetById(int id)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Get<TLetter>(id);
            }
        }
    }
}
