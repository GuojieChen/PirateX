﻿using PirateX.Core;
using PirateX.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Middleware
{
    public interface ISystemLetter
    {
        int Id { get; set; }
        /// <summary>
        /// 目标服列表
        /// </summary>
        List<int> TargetDidList { get; set; }
        /// <summary>
        /// 根据UID发放
        /// </summary>
        string[] UIDList { get; set; }
        /// <summary>
        /// 根据昵称发放
        /// </summary>
        string[] NameList { get; set; }
        /// <summary>
        /// 附件内容
        /// </summary>
        IReward Rewards { get; set; }
        /// <summary>
        /// 带有语言标识的标题 字典类型 KEY 为语言标识，Value为标题内容
        /// </summary>
        i18n[] i18nTitle { get; set; }
        /// <summary>
        /// 带有语言标识的内容 字典类型 KEY 为语言标识，Value为信件内容
        /// </summary>
        i18n[] i18nContent { get; set; }
    }

    public interface ISystemLetter<TLetter>
    where TLetter : ILetter
    {
        int Id { get; set; }
        /// <summary>
        /// 目标服列表
        /// </summary>
        int[] TargetDidList { get; set; }
        /// <summary>
        /// 根据UID发放
        /// </summary>
        string[] UIDList { get; set; }
        /// <summary>
        /// 根据昵称发放
        /// </summary>
        string[] NameList { get; set; }
        /// <summary>
        /// 附件内容
        /// </summary>
        TLetter Rewards { get; set; }
        /// <summary>
        /// 带有语言标识的标题 字典类型 KEY 为语言标识，Value为标题内容
        /// </summary>
        Dictionary<string, string> i18nTitle { get; set; }
        /// <summary>
        /// 带有语言标识的内容 字典类型 KEY 为语言标识，Value为信件内容
        /// </summary>
        Dictionary<string, string> i18nContent { get; set; }
    }
}
