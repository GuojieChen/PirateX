using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Actor.ProtoSync;
using ProtoBuf;

namespace PirateX.Core.Actor.System
{
    public class ProtoSync:RepAction<ProtoSyncResponse>
    {
        public override string Name => "_ProtoSync_";

        public override ProtoSyncResponse Play()
        {
            var hash = base.Context.Request.QueryString["hash"];
            var service = base.ServerReslover.Resolve<IProtoService>();

            if (Equals(hash, service.GetProtosHash()))
                return null; 

            return new ProtoSyncResponse()
            {
                Hash = service.GetProtosHash(),
                Proto = service.GetProto()
            };
        }
    }

    [ProtoContract(Name = "ProtoSyncResponse")]
    public class ProtoSyncResponse
    {
        [ProtoMember(1)]
        public string Hash { get; set; }

        [ProtoMember(2)]
        public string Proto { get; set; }
    }
}
