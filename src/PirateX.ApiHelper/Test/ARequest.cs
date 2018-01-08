using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PirateX.Core.Actor;
using ProtoBuf;

namespace PirateX.ApiHelper.Test
{
    [ApiDoc(Des = "请求")]
    [RequestDoc(Name = "name",Des = "名字",Type = "string")]
    public class ARequest:RepAction<ARequestResponse>
    {
        public override ARequestResponse Play()
        {
            throw new NotImplementedException();
        }
    }

    [ApiDoc(Des = "测试返回模型")]
    [ProtoContract]
    public class ARequestResponse
    {
        [ApiDoc(Des = "id")]
        [ProtoMember(1)]
        public int Id { get; set; }

        [ApiDoc(Des = "name")]
        [ProtoMember(2)]
        public string Name { get; set; }
    }
}