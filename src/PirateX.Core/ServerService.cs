using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NLog;
using PirateX.Core.Actor;
using PirateX.Core.Actor.ProtoSync;
using PirateX.Core.Broadcas;
using PirateX.Core.Container;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Core.Session;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;

namespace PirateX.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerService //: IMessageSender
    {
        /// <summary>
        /// 
        /// </summary>
        public ServerService(IDistrictContainer districtContainer)
        {
            DistrictContainer = districtContainer;
        }
        /// <summary>
        /// 
        /// </summary>
        protected ISessionManager OnlineManager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDistrictContainer DistrictContainer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 加载各种配置
        /// </summary>
        public virtual void ServerSetup()
        {
            var serverSetting = DistrictContainer.GetServerSetting();

            var configtypes = serverSetting.GetType().GetInterfaces();

            var builder = new ContainerBuilder();

            //默认在线管理  
            builder.Register(c => new MemorySessionManager())
                .As<ISessionManager>()
                .SingleInstance();

            foreach (var type in configtypes)
            {
                var attrs = type.GetCustomAttributes(typeof(ServerSettingRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                if (attrs[0] is ServerSettingRegisterAttribute attr)
                    ((IServerSettingRegister)Activator.CreateInstance(attr.RegisterType))
                        .Register(builder, serverSetting);
            }

            ////默认的包解析器
            builder.Register(c => new ProtocolPackage())
                .InstancePerDependency()
                .As<IProtocolPackage>();

            //默认消息广播
            builder.Register(c => new DefaultMessageBroadcast()).SingleInstance();

            builder.Register(c => new ProtobufService()).As<IProtoService>().SingleInstance();

            Setup(builder);

            DistrictContainer.InitContainers(builder);

            OnlineManager = DistrictContainer.ServerIoc.Resolve<ISessionManager>();
            if (Logger.IsTraceEnabled)
                Logger.Trace($"Set OnlineManager = {OnlineManager.GetType().FullName}");

            Setuped();
        }

        protected virtual void Setup(ContainerBuilder builder)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void Setuped()
        {
            
        }
    }
}
