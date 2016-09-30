using System;
using System.Collections.Generic;
using Autofac;

namespace PirateX.Core.Container
{
    public interface IServerContainer:IDisposable
    {
        /// <summary> 服务器容器
        /// </summary>
        ILifetimeScope ServerIoc { get; set; }

        IServerSetting Settings { get; }

        /// <summary> 获取游戏服容器
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        IContainer GetDistrictContainer(int districtid);
        /// <summary> 获取管理的配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDistrictConfig> GetDistrictConfigs();
        
        /// <summary>
        /// 初始化容器信息
        /// </summary>
        void InitContainers(ContainerBuilder builder);


        void ServerConfig(ContainerBuilder builder);

        IContainerSetting ContainerSetting { get; }


        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDistrictConfig> LoadDistrictConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IDistrictConfig GetDistrictConfig(int id);

        void BuildContainer(ContainerBuilder builder);



        /// <summary>
        /// 用到的连接字符串列表
        /// </summary>
        IDictionary<string, string> GetConnectionStrings();
            
        IDatabaseFactory GetConfigDatabaseFactory(IDistrictConfig config);

        IDatabaseFactory GetDistrictDatabaseFactory(IDistrictConfig config);
    }



}
