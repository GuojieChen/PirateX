using PirateX.GMSDK.Mapping;
using PirateX.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    public class i18n
    {
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }

    public class GMUILetterMap<TGMLetter> : GMUIItemMap<TGMLetter>
        where TGMLetter:ISystemLetter
    {
        public GMUILetterMap()
        {
            base.Name = "编辑信件";
            base.Des = "";

            Map<GMUIMapPropertyMap>(item => item.i18nTitle)
                .ToDisplayName("多语言标题")
                .ToPropertyMap(new GMUIi18nMap());

            Map<GMUIMapPropertyMap>(item => item.i18nContent)
                .ToDisplayName("多语言标题")
                .ToPropertyMap(new GMUIi18nMap());
        }
    }

    public class GMUIi18nMap : GMUIItemMap<i18n>
    {
        public GMUIi18nMap()
        {
            Map<GMUITextBoxPropertyMap>(item => item.Language)
                .ToDisplayName("语言");

            Map<GMUITextBoxPropertyMap>(item => item.Content)
                .ToDisplayName("内容");
        }
    }
}
