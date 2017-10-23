using PirateX.Core.Session;

namespace PirateX.Core.Actor
{
    public interface IMessageSender
    {
        void SendMessage<T>(ActorContext context, T t);

        void SendMessage<T>(ActorContext context, string name, T t);

        void SendMessage(ActorContext context, string name, string msg);

        void SendSeed<T>(ActorContext context, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, T t);

        void PushMessage<T>(int rid, T t);

        void PushMessage<T>(int rid, string name, T t);
    }
}
