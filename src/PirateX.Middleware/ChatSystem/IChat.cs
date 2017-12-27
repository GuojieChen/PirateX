using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware
{
    public interface IChat:IEntity<long>
        ,IEntityPrivate
        , IEntityCreateAt
        , IEntityDistrict
    {

    }
}
