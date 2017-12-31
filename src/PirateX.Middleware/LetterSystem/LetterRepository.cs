using System;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;
using PirateX.Core.Domain.Repository;

namespace PirateX.Middleware
{
    public class LetterRepository:RepositoryBase<ILetter>
    {
        public int Insert(ILetter letter)
        {
            return (int)base.DbConnection.Insert(letter);
        }

        public void Insert(IEnumerable<ILetter> letters)
        {
            base.DbConnection.Insert(letters);
        }

        public int Delete(long rid,int id)
        {
            //TODO 这里可以根据情况做下归档
            return base.DbConnection.Execute($"delete from letter where rid = {rid} and Id = {id}");
        }

        public List<TLetter> GetList<TLetter>(long rid, int page, int size = 50) where TLetter : ILetter
        {
            throw new NotImplementedException();
        }
    }
}
