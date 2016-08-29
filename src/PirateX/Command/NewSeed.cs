using System;
using PirateX.Core.Utils;
using PirateX.Filters;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using ProtoBuf;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    /// <summary>
    /// 客户端服务端种子交换操作
    /// </summary>
    [SeedCreatedFilter]
    public class NewSeed<TSession> : GameCommand<TSession, NewSeedRequestAndResponse, NewSeedRequestAndResponse>
        where TSession : PirateXSession<TSession>, IAppSession<TSession, IPirateXRequestInfo>, new()
    {
        public override string Name => "NewSeed"; 

        protected override NewSeedRequestAndResponse ExecuteResponseCommand(TSession session, NewSeedRequestAndResponse data)
        {
            var serverSeed = (int)(TimeUtil.GetTimestamp(new DateTime(DateTime.Now.Ticks + RandomUtil.Random.Next(-10001, 10001))) / 1000);

            var clientKey = new KeyGenerator(data.Seed);
            var serverKey = new KeyGenerator(serverSeed);

            session.ProtocolPackage.ClientKeys.Add(clientKey.MakeKey());
            session.ProtocolPackage.ServerKeys.Add(serverKey.MakeKey());

            return new NewSeedRequestAndResponse()
            {
                Seed = serverSeed
            };
        }

    }

    [ProtoContract]
    public class NewSeedRequestAndResponse
    {
        [ProtoMember(1)]
        public int Seed { get; set; }
    }

    public class KeyGenerator
    {
        public int Seed { get; private set; }

        public KeyGenerator(int seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// 随机算法
        /// </summary>
        /// <returns></returns>
        public int Rand()
        {
            Seed = Seed * 0x343fd + 0x269EC3;  // a=214013, b=2531011
            return (Seed >> 0x10) & 0x7FFF;
        }

        public byte[] MakeKey()
        {
            const int tableSize = 128;
            var originals = new int[tableSize];

            for (var i = 0; i < tableSize; i++)
            {
                originals[i] = i;
            }

            for (var i = 0; i < tableSize; i++)
            {
                var rand = ((char)(Rand() % tableSize));

                var tmp = originals[i];
                originals[i] = originals[rand];
                originals[rand] = tmp;
            }

            //size [4,16)
            var size = Rand() % 12 + 4;
            var keys = new byte[size];

            for (var i = 0; i < size; i++)
            {
                keys[i] = (byte)originals[i];
            }

            return keys;
        }
    }
}
