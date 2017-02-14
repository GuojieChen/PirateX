using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Sync.ProtoSync;

namespace PirateX.Net.Actor.Actions.System
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
