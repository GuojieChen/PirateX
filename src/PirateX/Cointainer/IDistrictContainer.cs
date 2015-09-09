using System.Collections.Generic;
using Autofac;

namespace PirateX.Cointainer
{
    public interface IDistrictContainer<TDistrictConfig> where TDistrictConfig : IDistrictConfig
    {
        /// <summary> 服务器容器
        /// </summary>
        ILifetimeScope ServerIoc { get; set; }

        /// <summary> 获取游戏服容器
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        IContainer GetDistrictContainer(int districtid);
        /// <summary>
        /// 重新加载配置
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        IContainer LoadDistrictContainer(int districtid);
        /// <summary> 获取管理的配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TDistrictConfig> GetDistrictConfigs();
        /// <summary>
        /// 初始化容器信息
        /// </summary>
        /// <param name="districtids">启动就初始化的游戏服ID列表</param>
        void InitContainers(params int[] districtids);


        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TDistrictConfig> LoadDistrictConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDistrictConfig GetDistrictConfig(int id);

        void BuildContainer(ContainerBuilder builder, TDistrictConfig config);
    }



}
