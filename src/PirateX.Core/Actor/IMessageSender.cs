namespace PirateX.Core
{
    public interface IMessageSender
    {
        byte[] SendMessage<T>(ActorContext context, T t);

        byte[] SendMessage<T>(ActorContext context, string name, T t);

        byte[] SendMessage(ActorContext context, string name, string msg);

        byte[] SendSeed<T>(ActorContext context, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, T t);

        void PushMessage<T>(int rid, T t);

        void PushMessage<T>(int rid, string name, T t);
    }
}
