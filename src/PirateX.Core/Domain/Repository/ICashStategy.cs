using System.Collections.Generic;

namespace PirateX.Core
{
    //TODO ,可以对基本对象的一个简单抽象

    public interface ICashStategy
    {
        void Set<T>(T t) where T : IEntity,IEntityPrivate;

        T Get<T>(object id) where T : IEntity, IEntityPrivate;

        IEnumerable<T> GetList<T>(long rid) where T : IEntity, IEntityPrivate;
    }
}
