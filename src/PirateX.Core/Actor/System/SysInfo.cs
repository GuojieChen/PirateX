using Autofac;
using ProtoBuf;
using PirateX.Core.Actor.ProtoSync;

namespace PirateX.Core.Actor.System
{
    public class SysInfo:RepAction<SystemInfoResponse>
    {
        public override string Name => "_sysinfo";

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
        [ProtoMember(1)]
        public string ProtoHash { get; set; } 
    }
}
