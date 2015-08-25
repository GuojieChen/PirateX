using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using PokemonX.Common;
using PokemonX.Common.Exception;
using PokemonX.Common.Response;
using PokemonX.StatusCode;
using PokemonX.SuperSocket.Protocol;
using ServiceStack;
using ServiceStack.Redis;

namespace PokemonX.ProxyServer.Cmd
{
    public class NewSeedRequest
    {
        public long Seed { get; set; }
    }

    public class NewSeed : JsonSubCommand<PokemonSession, NewSeedRequest, ResponseBase>
    {

        protected override ResponseBase ExecuteResponseCommand(PokemonSession session, NewSeedRequest data)
        {
            var serverSeed = Utils.GetTimestampAsSecond();

            var clientKey = new KeyGenerator(Convert.ToInt32(data.Seed));
            var serverKey = new KeyGenerator(serverSeed);

            //用客户端的seed生成一个seed
            //保存秘钥
            session.PackageProcessor.ClientKeys.Add(clientKey.MakeKey());
            session.PackageProcessor.ServerKeys.Add(serverKey.MakeKey());

            //返回数据
            session.SendMessage(new
            {
                C = Name,
                //M = requestInfo.M,
                D = new { Seed = serverSeed }
            });

            session.PackageProcessor.CryptoEnable = true;

            session.ConnectTarget(ProxyConnectedHandle);

            return null;
        }


        private void ProxyConnectedHandle(PokemonSession session, global::SuperSocket.ClientEngine.TcpClientSession targetSession)
        {
            //if (targetSession == null)
            //    session.SendResponse(m_FailedResponse, 0, m_FailedResponse.Length);
            //else
            //    session.SendResponse(m_OkResponse, 0, m_OkResponse.Length);
        }
    }
}
