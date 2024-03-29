﻿using PirateX.Core;
using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.GMSDK.Demo.Domain;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.Domain
{

    public class Letter : ILetter
    {
        public int FromRid { get; set; }
        public string FromName { get; set; }
        public int TemplateId { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public bool IsRead { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public i18n[] i18NTitle { get; set; }
        public i18n[] i18nContent { get; set; }
        public int Id { get; set; }
        public int Rid { get; set; }
        public string CreateAt { get; set; }
    }

    public class SystemLetter : ISystemLetter
    {
        public int Id { get; set; }
        public List<int> TargetDidList { get; set; }
        public string[] UIDList { get; set; }
        public string[] NameList { get; set; }
        public Reward Rewards { get; set; }
        public i18n[] i18nTitle { get; set; }
        public i18n[] i18nContent { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime EndAt { get; set; }

        public TLetter ToLetter<TLetter>(int rid) where TLetter : ILetter
        {
            return default(TLetter); 

            //return new Letter();
        }
    }


    public class GMUISystemLetterMap : GMUIItemMap<SystemLetter>
    {
        public GMUISystemLetterMap()
        {
            base.Name = "编辑信件";
            base.Des = "";

            Map<GMUIMapPropertyMap>(item => item.i18nTitle)
                .ToDisplayName("多语言标题")
                .ToPropertyMap(new GMUIi18nMap());

            Map<GMUIMapPropertyMap>(item => item.i18nContent)
                .ToDisplayName("多语言内容")
                .ToPropertyMap(new GMUIi18nMap());

            Map<GMUITextAreaPropertyMap>(item => item.NameList)
                .ToDisplayName("昵称列表")
                .ToTips("一行一个");

            Map<GMUITextAreaPropertyMap>(item => item.UIDList)
                .ToDisplayName("UID列表")
                .ToTips("一行一个");

            Map<GMUIDropdownPropertyMap>(item => item.Rewards)
                .ToDisplayName("选择奖励")
                .ToTips("")
                .ToListDataProvider(GMUIRewardListDataProvider.Instance);
        }
    }

    public class GMUIAllSystemLetterMap : GMUIItemMap<SystemLetter>
    {
        public GMUIAllSystemLetterMap()
        {
            base.Name = "编辑信件";
            base.Des = "";

            Map<GMUIListCheckBoxPropertyMap>(item => item.TargetDidList)
                .ToDisplayName("选择服")
                .ToTips("xxxxx")
                .ToGroupName("目标服")
                .ToCheckedDataProvider(GMUIDistrictListDataProvder.Instance);

            Map<GMUIMapPropertyMap>(item => item.i18nTitle)
                .ToDisplayName("多语言标题")
                .ToPropertyMap(new GMUIi18nMap());

            Map<GMUIMapPropertyMap>(item => item.i18nContent)
                .ToDisplayName("多语言内容")
                .ToPropertyMap(new GMUIi18nMap());

            Map<GMUITextAreaPropertyMap>(item => item.NameList)
                .ToDisplayName("昵称列表")
                .ToTips("一行一个");

            Map<GMUITextAreaPropertyMap>(item => item.UIDList)
                .ToDisplayName("UID列表")
                .ToTips("一行一个");

            Map<GMUIDropdownPropertyMap>(item => item.Rewards)
                .ToDisplayName("选择奖励")
                .ToTips("")
                .ToListDataProvider(GMUIRewardListDataProvider.Instance);
        }
    }
}
