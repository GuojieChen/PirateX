using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace PirateX.Core.Container
{
    public interface IDistrictContainer:IDisposable
    {
        /// <summary> 服务器容器
        /// </summary>
        ILifetimeScope ServerIoc { get; set; }

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
        
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IDistrictConfig GetDistrictConfig(int id);

        /// <summary>
        /// 用到的连接字符串列表
        /// 
        /// 可以通过全局容器或者服容器获取相应的链接
        /// Reslover.KeyedResolveNamed&lt;IDbConnection&gt;(name)
        /// </summary>
        IDictionary<string, string> GetNamedConnectionStrings();

        List<Assembly> GetEntityAssemblyList();

        List<Assembly> GetConfigAssemblyList();

        List<Assembly> GetServiceAssemblyList();
        List<Assembly> GetApiAssemblyList();

        List<Assembly> GetRepositoryAssemblyList();

        IServerSetting GetServerSetting();
    }



}
