using System;
using System.Collections.Generic;
using System.Data;
using Autofac;
using Dapper;
using Dapper.Contrib.Extensions;
using PirateX.Core.Domain.Repository;

namespace PirateX.Middleware.LetterSystem
{
    public class LetterRepository:RepositoryBase
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

        public virtual int Delete(long rid,int id)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Execute($"delete from letter where rid = {rid} and Id = {id}");
            }

            //TODO 这里可以根据情况做下归档
        }

        public virtual List<TLetter> GetList<TLetter>(long rid, int page, int size = 50) where TLetter : ILetter
        {
            throw new NotImplementedException();
        }
    }
}
