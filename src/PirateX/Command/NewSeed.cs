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

            session.ProtocolPackage.ClientKeys = clientKey.MakeKey();
            session.ProtocolPackage.ServerKeys = serverKey.MakeKey();

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
}
