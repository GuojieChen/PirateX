using System.Collections.Specialized;

namespace PirateX.Core
{
    public interface IActorNetService
    {
        void Start();
        void Stop();

        void PushMessage(int rid,NameValueCollection headers,byte[] body);

        byte[] Seed(ActorContext context, NameValueCollection header, byte cryptobyte, byte[] clientkeys,
            byte[] serverkeys, byte[] body);

        byte[] SendMessage(ActorContext context, NameValueCollection headers, byte[] body);
    }
}
