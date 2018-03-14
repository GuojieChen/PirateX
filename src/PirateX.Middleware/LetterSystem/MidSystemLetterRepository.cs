using Autofac;
using PirateX.Core;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Middleware
{
    public class MidSystemLetterRepository<TSystemLetter>: PublicRepository
        where TSystemLetter : class, ISystemLetter
    {

        #region SystemLetter

        public virtual void SendSystemLetter(TSystemLetter letter)
        {
            using (var db = DbConnection())
            {
                db.Insert<TSystemLetter>(letter);
            }
        }

        public IEnumerable<TSystemLetter> GetSystemLetters()
        {
            using (var db = DbConnection())
            {
                return db.Query<TSystemLetter>($"select * from {typeof(TSystemLetter).Name} where OpenAt>=@Now or EndAt<@Now", new { now = DateTime.UtcNow });
            }
        }

        #endregion
    }
}
