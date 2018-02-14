using PirateX.Core;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    public class GMUILetterMap<TGMLetter> : GMUIItemMap<TGMLetter>
        where TGMLetter:ISystemLetter<IReward>
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

            Map<GMUITextAreaPropertyMap>(item => item.NameList)
                .ToDisplayName("昵称列表")
                .ToTips("一行一个");

            Map<GMUITextAreaPropertyMap>(item => item.UIDList)
                .ToDisplayName("UID列表")
                .ToTips("一行一个");
        }
    }

    public class GMUIi18nMap : GMUIItemMap<i18n>
    {
        public GMUIi18nMap()
        {
            Map<GMUITextBoxPropertyMap>(item => item.Language)
                .ToDisplayName("语言");

            Map<GMUITextAreaPropertyMap>(item => item.Content)
                .ToDisplayName("内容");
        }
    }
}
