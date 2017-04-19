using PirateX.Core.Online;

namespace PirateX.Core.Actor
{
    public interface IMessageSender
    {
        void SendMessage<T>(ActorContext context, T t);

        void SendMessage<T>(ActorContext context, string name, T t);

        void PushMessage<T>(IOnlineRole role,T t);
    }
}
