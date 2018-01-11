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
    /// <summary>
    /// proto描述同步
    /// </summary>
    [RequestDoc(Des = "上次同步后的hash值",Name = "hash",Type = typeof(string))]
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

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    public class ProtoSyncResponse
    {
        [ProtoMember(1)]
        public string Hash { get; set; }

        [ProtoMember(2)]
        public byte[] Proto { get; set; }
    }
}
