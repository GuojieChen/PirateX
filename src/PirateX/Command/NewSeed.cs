using System;
using PirateX.Core.Utils;
using PirateX.Filters;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    /// <summary>
    /// 客户端服务端种子交换操作
    /// </summary>
    [SeedCreatedFilter]
    public class NewSeed<TSession> : GameCommand<TSession, NewSeedRequestAndResponse, NewSeedRequestAndResponse>
        //CommandBase<TSession,GameRequestInfoV1> 
        where TSession : GameSession<TSession>, IAppSession<TSession, IGameRequestInfo>, new()
    {
        protected override NewSeedRequestAndResponse ExecuteResponseCommand(TSession session, NewSeedRequestAndResponse data)
        {
            var serverSeed = (int)(TimeUtil.GetTimestamp(new DateTime(DateTime.Now.Ticks + RandomUtil.Random.Next(-10001, 10001))) / 1000);

            var clientKey = new KeyGenerator(data.Seed);
            var serverKey = new KeyGenerator(serverSeed);

            //用客户端的seed生成一个seed
            //保存秘钥
            if (Equals(data.Format.ToUpper(), "JSON"))
                session.ProtocolPackage.JsonEnable = true;

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

        public string Format { get; set; }
    }
}
