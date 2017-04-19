using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using GameServer.Console.SampleConfig;
using GameServer.Console.SampleService;
using PirateX.Core;
using PirateX.Core.Actor;
using PirateX.Core.Config;
using PirateX.Core.Online;
using ProtoBuf;

namespace GameServer.Console.Cmd
{
    public class RoleLoginV2 : Login<RoleLoginV2Response>
    {

        public override RoleLoginV2Response Play()
        {
            Logger.Error(Reslover.Resolve<IConfigReader>().SingleById<PetConfig>(1).Name);
            var roleservice = Reslover.Resolve<RoleService>();

            Logger.Error(roleservice);
            roleservice.ShowLog();

            return new RoleLoginV2Response()
            {
                Token = "xxxx",
                Rid = 1, 
                Did = 2 ,
            };
        }

        public override IOnlineRole GetOnlineRole()
        {
            return new OnlineRole
            {
                //Id = Context.Request.Headers.
                //Did = request.Did,
                //SessionId = session.SessionID
            };
        }
    }

    [Serializable]
    [ProtoContract]
    public class RoleLoginV2Response 
    {
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public long Rid { get; set; }
        [ProtoMember(3)]
        public int Did { get; set; }
    }
}