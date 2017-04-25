using System;
using System.Collections.Specialized;
using PirateX.Core.Session;

namespace PirateX.Core.Actor
{
    public interface IActorNetService
    {
        void Start();
        void Stop();

        void PushMessage(PirateSession role,NameValueCollection headers,byte[] body);

        void SendMessage(ActorContext context, NameValueCollection headers, byte[] body);
    }
}
