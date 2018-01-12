using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNanomsg.Protocols;
using PirateX.Core.Actor;
using PirateX.Core.Session;
using PirateX.Core.Utils;

namespace PirateX.Net.NNanomsg
{
    public class ActorNetService : IActorNetService
    {
        private ActorConfig config;
        private IActorService _actorService;

        private PullSocket PullSocket { get; set; }
        private PushSocket PushSocket { get; set; }

        public ActorNetService(IActorService actorService, ActorConfig config)
        {
            this._actorService = actorService;
            _actorService.NetService = this;
            this.config = config;
        }

        private void SetUp()
        {
            _actorService.Setup();

            PullSocket = new PullSocket();

            PushSocket = new PushSocket();
        }

        public void Start()
        {
            SetUp();

            _actorService.Start();

            PushSocket.Bind(config.PushsocketString);
            PullSocket.Bind(config.PullSocketString);
        }

        public void Stop()
        {
            _actorService.Stop();
        }

        public void PushMessage(int rid, NameValueCollection headers, byte[] body)
        {
            throw new NotImplementedException();
        }

        public void PushMessage(string sessionid, NameValueCollection headers, byte[] body)
        {
            throw new NotImplementedException();
        }

        public void Seed(ActorContext context, NameValueCollection header, byte cryptobyte, byte[] clientkeys, byte[] serverkeys,
            byte[] body)
        {
            throw new NotImplementedException();
        }

        public void PushMessage(PirateSession role, NameValueCollection headers, byte[] body)
        {
            PushSocket.SendImmediate(new GmaeMail()
            {
                Version = 1,
                Action = 1,
                SessionId = role.SessionId,
                ClientKeys = role.ClientKeys,
                ServerKeys = role.ServerKeys,
                Headers = GetHeaderBytes(headers),
                Body = body

            }.ToProtobuf());
        }

        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }
        public void SendMessage(ActorContext context, NameValueCollection headers, byte[] body)
        {
            PushSocket.SendImmediate(new GmaeMail()
            {
                Version = 1,
                Action = 1,
                Headers = GetHeaderBytes(headers),
                Body = body

            }.ToProtobuf());
        }
    }
}
