using PirateX.Core;

namespace PirateX.Middleware
{
    public interface IChat:
         IEntityPrivate
        , IEntityCreateAt
        , IEntityDistrict
    {
        long Id { get; set; }

        /// <summary>
        /// 渠道，可以定义为私人，世界，公会等
        /// </summary>
        int ChannelId { get; set; }

        string Content { get; set; }
    }
}
