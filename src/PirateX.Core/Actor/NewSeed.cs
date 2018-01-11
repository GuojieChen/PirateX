using System;
using System.Collections;
using PirateX.Protocol;
using ProtoBuf;
using RandomUtil = PirateX.Core.Utils.RandomUtil;

namespace PirateX.Core.Actor
{
    /// <summary>
    /// 种子交换
    /// </summary>
    [RequestDoc(Name = "seed", Des = "客户端种子", Type = typeof(int))]
    public class NewSeed : RepAction
    {
        private static readonly bool[] CryptoByte = new bool[8]
        {
            false, false, false, false,
            false, false, false, true
        };
        public override void Execute()
        {
            var seed = Convert.ToInt32(Context.Request.QueryString["seed"]);

            var serverSeed = (int)(new DateTime(DateTime.Now.Ticks + RandomUtil.Random.Next(-10001, 10001)).GetTimestamp() / 1000);

            var ClientKeys = new KeyGenerator(seed).MakeKey();
            var ServerKeys = new KeyGenerator(serverSeed).MakeKey();
            var cryptobyte = new BitArray(CryptoByte).ConvertToByte();

            base.MessageSender.SendSeed(base.Context, cryptobyte, ClientKeys, ServerKeys, new NewSeedResponse()
            {
                Seed = serverSeed
            });


        }

    }


    [ProtoContract]
    public class NewSeedResponse
    {
        /// <summary>
        /// 服务器种子
        /// </summary>
        [ProtoMember(1)]
        public int Seed { get; set; }
    }
}
