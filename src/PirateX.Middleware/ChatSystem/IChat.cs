using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
{
    public interface IChat:
         IEntityPrivate
        , IEntityCreateAt
        , IEntityDistrict
    {
        string Id { get; set; }

        /// <summary>
        /// 渠道，可以定义为私人，世界，公会等
        /// </summary>
        int ChannelId { get; set; }
        /// <summary>
        /// 系统信件的模板ID
        /// </summary>
        int TemplateId { get; set; }
        /// <summary>
        /// 系统信件的数据
        /// </summary>
        Dictionary<string,string> Values { get; set; }
        /// <summary>
        /// 玩家发的信息
        /// </summary>
        string Content { get; set; }
    }
}
