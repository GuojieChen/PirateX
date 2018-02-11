using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
{
    public interface ILetter:IEntity<int>,IEntityPrivate,IEntityCreateAt
    {
        /// <summary>
        /// 发送方角色ID 
        /// 0 的时候表示系统发送.此时内容被定义为模板，通过TemplateId获取具体模板  Values为具体的值列表
        /// 小于0的时候表示由管理员发送，并且Math.Abs(FromRid) 为管理员的UC id
        /// </summary>
        int FromRid { get; set; }
        /// <summary>
        /// 发送放昵称，如果系统发送，则不设置昵称
        /// </summary>
        string FromName { get; set; }
        /// <summary>
        /// 模板ID 小于0的情况下标识信件由GM工具发送，并且指定了模板。模板可以是字典形{"language":"","content":""}。
        /// 客户端获取的时候由服务器填充Content字段下发给客户端。
        /// </summary>
        int TemplateId { get; set; }
        /// <summary>
        /// 信件内容
        /// 可以是字典类型，用以刷新信件模板（按照信件TemplateId来定位模板）
        /// 实现的类中可以自己决定Values的序列化和反序列化规则
        /// </summary>
        Dictionary<string,string> Values { get; set; }
        /// <summary>
        /// 已读
        /// </summary>
        bool IsRead { get; set; }
        /// <summary>
        /// 玩家信件时候 具体的内容
        /// </summary>
        string Content { get; set; }
    }
}
