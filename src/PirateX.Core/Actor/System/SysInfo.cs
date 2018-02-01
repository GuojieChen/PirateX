using Autofac;
using ProtoBuf;

namespace PirateX.Core
{
    /// <summary>
    /// 获取系统信息
    /// </summary>
    public class SysInfo:RepAction<SystemInfoResponse>
    {
        public override string Name => "_sysinfo_";

        public override SystemInfoResponse Play()
        {
            var protoservice = base.ServerReslover.Resolve<IProtoService>();
            return new SystemInfoResponse()
            {
                ProtoHash = protoservice.GetProtosHash()
            };
        }
    }

    [ProtoContract]
    public class SystemInfoResponse
    {
        /// <summary>
        /// proto hash
        /// </summary>
        [ProtoMember(1)]
        public string ProtoHash { get; set; } 
    }
}
