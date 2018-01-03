using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;

namespace PirateX.Core.Domain.Repository
{
    //TODO ,可以对基本对象的一个简单抽象

    public interface ICashStategy
    {
        void Set<T>(T t) where T : IEntity,IEntityPrivate;

        T Get<T>(object id) where T : IEntity, IEntityPrivate;

        IEnumerable<T> GetList<T>(long rid) where T : IEntity, IEntityPrivate;
    }
}
