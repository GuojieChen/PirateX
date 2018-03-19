using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.ActivityDataItems
{
    public class TimeCopyActive
    {
        /// <summary> 主打精灵1 </summary>
        public int PetId1 { get; set; }

        /// <summary> 主打精灵1 基础捕捉概率 </summary>
        public double BaseRate1 { get; set; }

        /// <summary> 主打精灵1 每次捕捉失败增加概率 </summary>
        public double AddRate1 { get; set; }

        /// <summary> 主打精灵2 </summary>
        public int PetId2 { get; set; }

        /// <summary> 主打精灵2 基础捕捉概率 </summary>
        public double BaseRate2 { get; set; }

        /// <summary> 主打精灵2 每次捕捉失败增加概率 </summary>
        public double AddRate2 { get; set; }

        public TimeCopyStageCfg[] StageCfgs { get; set; }

        /// <summary> 进入条件类型 1：要求精灵属性；2：要求精灵职业 Define.TimeCopyConType </summary>
        public int ConType { get; set; }

        /// <summary> 进入条件值 属性对应Define.PropId, 职业对应职业 Define.ClassId </summary>
        public int ConValue { get; set; }

        /// <summary> 兑换项目 </summary>
        public ExItem[] ExItems { get; set; }
    }

    public class GMUITimeCopyActivePropertyMap : GMUIItemMap<TimeCopyActive>
    {
        public GMUITimeCopyActivePropertyMap()
        {
            base.Name = "限时副本";
            base.Des = "限时副本xxxxxxx"; 

            Map<GMUIDropdownPropertyMap>(item=>item.PetId1)
                .ToDisplayName("精灵1")
                .ToGroupName("主打精灵1")
                .ToListDataProvider(GMUIPetListProvider.Instance);
            Map<GMUITextBoxPropertyMap>(item => item.BaseRate1)
                .ToDisplayName("基础捕捉概率")
                .ToGroupName("主打精灵1");
            Map<GMUITextBoxPropertyMap>(item => item.AddRate1)
                .ToDisplayName("递增概率")
                .ToDevaultValue("50")
                .ToTips("xxxxxxxx")
                .ToGroupName("主打精灵1");

            Map<GMUIDropdownPropertyMap>(item => item.PetId2)
                .ToDisplayName("精灵")
                .ToGroupName("主打精灵2")
                .ToListDataProvider(GMUIPetListProvider.Instance);
            Map<GMUITextBoxPropertyMap>(item => item.BaseRate2)
                .ToDisplayName("基础捕捉概率")
                .ToGroupName("主打精灵2");
            Map<GMUITextBoxPropertyMap>(item => item.AddRate2)
                .ToDisplayName("递增概率")
                .ToGroupName("主打精灵2");

            Map<GMUIDropdownPropertyMap>(item => item.ConType)
                .ToDisplayName("进入条件类型")
                .ToGroupName("条件")
                .ToListDataProvider(GMUIConTypeListDataProvider.Instance);
            Map<GMUIDropdownPropertyMap>(item => item.ConValue)
                .ToDisplayName("进入条件值")
                .ToGroupName("条件")
                .ToListDataProvider(GMUIConValueListDataProvider.Instance);

            Map<GMUIMapPropertyMap>(item => item.StageCfgs)
                .ToDisplayName("关卡配置")
                .ToPropertyMap(new GMUITimeCopyStageCfgMap());

            Map<GMUIMapPropertyMap>(item => item.ExItems)
                .ToDisplayName("兑换项目")
                .ToPropertyMap(new GMUIExItemMap());
        }
    }

    public class TimeCopyStageCfg
    {
        /// <summary> GM 工具自动从1递增，不要手动填写 </summary>
        public int StageId { get; set; }

        /// <summary> 关卡名称 </summary>
        public string StageName { get; set; }

        /// <summary> NPC 对应精灵ID列表 </summary>
        public IList<int> PetIdList { get; set; }

        /// <summary> 每一只NPC精灵的生命 </summary>
        public int Hp { get; set; }

        /// <summary> 每一只NPC精灵的攻击 </summary>
        public int Atk { get; set; }
    }

    public class GMUITimeCopyStageCfgMap : GMUIItemMap<TimeCopyStageCfg>
    {
        public GMUITimeCopyStageCfgMap()
        {
            Map<GMUITextBoxPropertyMap>(item=>item.StageName)
                .ToDisplayName("关卡名称");
            Map<GMUITextBoxPropertyMap>(item => item.Hp)
                .ToDisplayName("NPC生命");
            Map<GMUITextBoxPropertyMap>(item => item.Atk)
                .ToDisplayName("NPC攻击");

            //
        }
    }

    public class ExItem
    {
        public int Id { get; set; }

        /// <summary> 单次消耗积分 </summary>
        public int Score { get; set; }

        /// <summary> 兑换次数上限 </summary>
        public int Times { get; set; }

        /// <summary> 兑换内容 </summary>
        //public Reward Reward { get; set; }
    }

    public class GMUIExItemMap : GMUIItemMap<ExItem>
    {
        public GMUIExItemMap()
        {
            Map<GMUIDropdownPropertyMap>(item => item.Id)
                .ToDisplayName("奖励")
                .ToListDataProvider(GMUIPetListProvider.Instance);

            Map<GMUITextBoxPropertyMap>(item => item.Score)
                .ToDisplayName("消耗积分");
            Map<GMUITextBoxPropertyMap>(item => item.Times)
                .ToDisplayName("次数上限");
        }
    }
}
