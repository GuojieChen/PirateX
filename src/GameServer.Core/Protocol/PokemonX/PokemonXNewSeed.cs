using System;
using GameServer.Core.Filters;
using GameServer.Core.Utils;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;

namespace GameServer.Core.Protocol.PokemonX
{
    /// <summary>
    /// 客户端服务端种子交换操作
    /// </summary>
    [SeedCreatedFilter]
    public class NewSeed<TSession> : CommandBase<TSession,IPokemonXRequestInfo> 
        where TSession : PSession<TSession,Enum>, IAppSession<TSession, IPokemonXRequestInfo>, new()
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 

        public override void ExecuteCommand(TSession session, IPokemonXRequestInfo requestInfo)
        {
            var data = requestInfo.Body["Seed"];

            if (data == null)
            {
                session.Close();
                return;
            }//throw new PokemonXException(ServerCode.BadRequest,"Seed error");


            var serverSeed = (int)(TimeUtil.GetTimestamp(new DateTime(DateTime.Now.Ticks + RandomUtil.Random.Next(-10001, 10001))) / 1000); 

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
