using PirateX.Core;
using PirateX.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Middleware
{
    public interface ISystemLetter:IEntity<int>
    {
        int Id { get; set; }
        /// <summary>
        /// 带有语言标识的标题 字典类型 KEY 为语言标识，Value为标题内容
        /// </summary>
        i18n[] i18nTitle { get; set; }
        /// <summary>
        /// 带有语言标识的内容 字典类型 KEY 为语言标识，Value为信件内容
        /// </summary>
        i18n[] i18nContent { get; set; }
        /// <summary>
        /// 开始时间(UTC)
        /// </summary>
        DateTime OpenAt { get; set; }
        /// <summary>
        /// 结束时间(UTC)
        /// </summary>
        DateTime EndAt { get; set; }
        /// <summary>
        /// 转成玩家信件
        /// </summary>
        /// <returns></returns>
        TLetter ToLetter<TLetter>(int rid) where TLetter :ILetter;
    }
}
