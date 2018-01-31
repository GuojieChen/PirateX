using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.GMSDK.Mapping;

namespace PirateX.GMSDK
{
    public interface IGMSDKService
    {
        /// <summary>
        /// 游戏列表
        /// </summary>
        IDistrictContainer DistrictContainer { get; }

        /// <summary>
        /// 获取子菜单列表
        /// </summary>
        /// <returns></returns>
        GMUINav[] GmuiNavs { get; }

        ContainerBuilder InitContainerBuilder();

        IGMUIItemMap[] GetActivityMaps();

        IGMRepository GetGmRepository();

        /// <summary>
        /// 获取奖励类型的映射
        /// </summary>
        /// <returns></returns>
        IGMUIItemMap GetRewardItemMap();
    }
}
