﻿using Autofac;
using ProtoBuf;

namespace PirateX.Core
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
        /// <summary>
        /// hash值
        /// </summary>
        [ProtoMember(1)]
        public string Hash { get; set; }
        /// <summary>
        /// proto内容
        /// </summary>
        [ProtoMember(2)]
        public byte[] Proto { get; set; }
    }
}
