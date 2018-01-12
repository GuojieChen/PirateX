using System;
using System.Collections.Generic;
using System.Data;
using Autofac;
using Dapper;
using Dapper.Contrib.Extensions;
using PirateX.Core.Domain.Repository;

namespace PirateX.Middleware
{
    public class LetterRepository:RepositoryBase
    {
        public int Insert(ILetter letter)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return (int)db.Insert(letter);
            }
        }

        public void Insert(IEnumerable<ILetter> letters)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                db.Insert(letters);
            }
        }

        public int Delete(long rid,int id)
        {
            using (var db = Resolver.Resolve<IDbConnection>())
            {
                return db.Execute($"delete from letter where rid = {rid} and Id = {id}");
            }

            //TODO 这里可以根据情况做下归档
        }

        public List<TLetter> GetList<TLetter>(long rid, int page, int size = 50) where TLetter : ILetter
        {
            throw new NotImplementedException();
        }
    }
}
