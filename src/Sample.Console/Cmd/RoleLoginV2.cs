using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PirateX.Command;
using PirateX.Core.Online;

namespace GameServer.Console.Cmd
{
    public class RoleLoginV2 : Login<DemoSession, DistrictConfig, RoleLoginV2Request, RoleLoginV2Request,OnlineRole>
    {
        public override RoleLoginV2Request DoLogin(DemoSession session, RoleLoginV2Request request)
        {
            Logger.Debug(Thread.CurrentThread.CurrentCulture);

            Task.Factory.StartNew(() => Logger.Debug(Thread.CurrentThread.CurrentCulture));

            return request;
        }

        public override OnlineRole GetOnlineRole(DemoSession session, RoleLoginV2Request request)
        {
            return new OnlineRole
            {
                Id = request.Rid,
                Did = request.Did,
                HotName = Dns.GetHostName(),
                SessionID = session.SessionID
            };
        }

        public override IToken UnPackToken(RoleLoginV2Request data)
        {
            return new Token
            {
                Rid = data.Rid,
                DistrictId = data.Did,
                Timestamp = 1000000000,
                Secret = "ERLKCKJEJLBKPCDWLEKRHKJGKWE"
            };
        }
    }

    public class Token : IToken
    {
        public long Rid { get; set; }
        public int DistrictId { get; set; }
        public long Timestamp { get; set; }
        public string Secret { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class RoleLoginV2Request : ILoginRequest, ILoginResponse
    {
        public string Token { get; set; }
        public long Rid { get; set; }
        public int Did { get; set; }
    }
}