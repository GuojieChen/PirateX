using System;
using ProtoBuf;

namespace PirateX.Core
{
    /// <summary>
    /// ping
    /// </summary>
    public class Ping : RepAction<PingResponse>
    {
        public override PingResponse Play()
        {
            return new PingResponse()
            {
                Now = DateTime.UtcNow.FromDateTime()
            };
        }
    }

    [ProtoContract]
    public class PingResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        public string Now { get; set; }
    }
}
