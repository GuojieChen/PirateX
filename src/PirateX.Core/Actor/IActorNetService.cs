using System;
using System.Collections.Specialized;
using PirateX.Core.Online;

namespace PirateX.Core.Actor
{
    public interface IActorNetService
    {
        void Start();
        void Stop();

        void PushMessage(IOnlineRole role,NameValueCollection headers,byte[] body);

        void SendMessage(ActorContext context, NameValueCollection headers, byte[] body);
    }
}
