using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using GameServer.Console.SampleConfig;
using GameServer.Console.SampleService;
using PirateX.Command;
using PirateX.Core;
using PirateX.Core.Config;
using PirateX.Core.Online;
using PirateX.Filters;
using ServiceStack.Text;

namespace GameServer.Console.Cmd
{
    [LoginSuccessFilter]
    public class RoleLoginV2 : Login<DemoSession, DistrictConfig, RoleLoginV2Request, RoleLoginV2Request,OnlineRole>
    {
        public override RoleLoginV2Request DoLogin(DemoSession session, RoleLoginV2Request request)
        {
            Logger.Error(session.Build.Resolve<IConfigReader>().SingleById<PetConfig>(1).Name);
            var roleservice = session.Build.Resolve<RoleService>();

            Logger.Error(roleservice);
            roleservice.ShowLog();

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