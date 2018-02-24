using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;

namespace PirateX.GMSDK
{
    public interface IGMSDKService
    {
        /// <summary>
        /// 游戏列表
        /// </summary>
        IDistrictContainer DistrictContainer { get; }
        /// <summary>
        /// 重置
        /// </summary>
        void SetInstanceNull();

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
        /// <summary>
        /// 返回一个新的活动实例
        /// </summary>
        /// <returns></returns>
        IActivity GetActivityInstance();
        /// <summary>
        /// 获取奖励类型
        /// </summary>
        /// <returns></returns>
        Type GetRewardType();
    }
}
