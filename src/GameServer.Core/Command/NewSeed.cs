using System;
using GameServer.Core.Ex;
using GameServer.Core.Filters;
using GameServer.Core.PkProtocol;
using SuperSocket.SocketBase.Command;

namespace GameServer.Core.Command
{
    /// <summary>
    /// 客户端服务端种子交换操作
    /// </summary>
    [SeedCreatedFilter]
    public class NewSeed<TSession> : CommandBase<TSession,ISocketRequestInfo> where TSession : PSession<TSession>, new()
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 

        public override void ExecuteCommand(TSession session, ISocketRequestInfo requestInfo)
        {
            var data = requestInfo.Body["Seed"]; 

            if(data == null)
                throw new PException(ServerCode.BadRequest,"Seed error");

            var serverSeed = (int)(Utils.GetTimestamp(new DateTime(DateTime.Now.Ticks + Utils.Random.Next(-10001, 10001))) / 1000); 

            var clientKey = new KeyGenerator(Convert.ToInt32(data));
            var serverKey = new KeyGenerator(serverSeed);

            //用客户端的seed生成一个seed
            //保存秘钥
            session.PackageProcessor.ClientKeys.Add(clientKey.MakeKey());
            session.PackageProcessor.ServerKeys.Add(serverKey.MakeKey());
            //返回数据
            session.SendMessage(new
            {
                C = Name,
                O = session.CurrentO,
                D = new { Seed = serverSeed }
            });

            session.PackageProcessor.CryptoEnable = true;
        }
    }
}
