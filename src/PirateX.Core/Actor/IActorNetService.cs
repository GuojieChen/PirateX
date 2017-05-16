using System;
using System.Collections.Specialized;
using PirateX.Core.Session;

namespace PirateX.Core.Actor
{
    public interface IActorNetService
    {
        void Start();
        void Stop();

        void PushMessage(int rid,NameValueCollection headers,byte[] body);

        void Seed(ActorContext context, NameValueCollection header, byte cryptobyte, byte[] clientkeys,
            byte[] serverkeys, byte[] body);

        void SendMessage(ActorContext context, NameValueCollection headers, byte[] body);
    }
}
