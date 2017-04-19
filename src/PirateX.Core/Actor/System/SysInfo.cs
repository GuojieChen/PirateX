using Autofac;
using PirateX.Core.Actor.ProtoSync;

namespace PirateX.Core.Actor.System
{
    public class SysInfo:RepAction<SystemInfoResponse>
    {
        public override string Name => "_sysinfo";

        public override SystemInfoResponse Play()
        {
            var protoservice = base.Reslover.Resolve<IProtoService>();
            return new SystemInfoResponse()
            {
                ProtoHash = protoservice.GetProtosHash()
            };
        }
    }


    public class SystemInfoResponse
    {
        public string ProtoHash { get; set; } 
    }
}
