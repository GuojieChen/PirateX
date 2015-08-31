using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Core.Command;
using GameServer.Core.Online;

namespace GameServer.Console.Cmd
{
    public class RoleLoginV2 : Login<DemoSession, GameServerConfig, RoleLoginV2Request, RoleLoginV2Request>
    {
        public override RoleLoginV2Request DoLogin(DemoSession session, RoleLoginV2Request request)
        {
            Logger.Debug(Thread.CurrentThread.CurrentCulture);

            Task.Factory.StartNew(() => Logger.Debug(Thread.CurrentThread.CurrentCulture));

            return request;
        }

        public override IOnlineRole GetOnlineRole(DemoSession session, RoleLoginV2Request request)
        {
            return new OnlineRole
            {
                Id = request.Rid,
                ServerId = request.ServerId,
                ServerName = Dns.GetHostName().Trim('\''),
                SessionID = session.SessionID
            };
        }

        public override IToken UnPackToken(RoleLoginV2Request data)
        {
            return new Token
            {
                Rid = data.Rid,
                ServerId = data.ServerId,
                Timestamp = 1000000000,
                Secret = "ERLKCKJEJLBKPCDWLEKRHKJGKWE"
            };
        }
    }

    public class OnlineRole : IOnlineRole
    {
        public long Id { get; set; }
        public int ServerId { get; set; }
        public string ServerName { get; set; }
        public string SessionID { get; set; }
    }

    public class Token : IToken
    {
        public long Rid { get; set; }
        public int ServerId { get; set; }
        public long Timestamp { get; set; }
        public string Secret { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class RoleLoginV2Request : ILoginRequest, ILoginResponse
    {
        public string Token { get; set; }
        public long Rid { get; set; }
        public int ServerId { get; set; }
    }
}