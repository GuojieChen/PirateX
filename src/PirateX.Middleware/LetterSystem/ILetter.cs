using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware
{
    public interface ILetter:IEntity<int>,IEntityPrivate,IEntityCreateAt
    {
        /// <summary>
        /// 发送方角色ID
        /// </summary>
        long FromRid { get; set; }
        /// <summary>
        /// 发送放昵称，如果系统发送，则不设置昵称
        /// </summary>
        string FromName { get; set; }
        /// <summary>
        /// 模板ID
        /// </summary>
        int TemplateId { get; set; }
        /// <summary>
        /// 信件内容
        /// 可以是字典类型，用以刷新信件模板（按照信件TemplateId来定位模板）
        /// 实现的类中可以自己决定Values的序列化和反序列化规则
        /// </summary>
        string Values { get; set; }
        /// <summary>
        /// 已读
        /// </summary>
        bool IsRead { get; set; }
    }
}
