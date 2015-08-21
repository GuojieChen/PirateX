using System;
using GameServer.Core.Filters;
using GameServer.Core.Protocol;
using GameServer.Core.Protocol.V1;
using GameServer.Core.Utils;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Command
{
    /// <summary>
    /// 客户端服务端种子交换操作
    /// </summary>
    [SeedCreatedFilter]
    public class NewSeed<TSession> : GameCommand<TSession, NewSeedRequestAndResponse, NewSeedRequestAndResponse>
        //CommandBase<TSession,GameRequestInfoV1> 
        where TSession : PSession<TSession,Enum>, IAppSession<TSession, IGameRequestInfo>, new()
    {
        protected override NewSeedRequestAndResponse ExecuteResponseCommand(TSession session, NewSeedRequestAndResponse data)
        {
            var serverSeed = (int)(TimeUtil.GetTimestamp(new DateTime(DateTime.Now.Ticks + RandomUtil.Random.Next(-10001, 10001))) / 1000);

            var clientKey = new KeyGenerator(data.Seed);
            var serverKey = new KeyGenerator(serverSeed);

            //用客户端的seed生成一个seed
            //保存秘钥
            session.ProtocolPackage.ClientKeys.Add(clientKey.MakeKey());
            session.ProtocolPackage.ServerKeys.Add(serverKey.MakeKey());

            return new NewSeedRequestAndResponse()
            {
                Seed = serverSeed
            };
        }

    }

    public class NewSeedRequestAndResponse
    {
        public int Seed { get; set; }
    }
}
