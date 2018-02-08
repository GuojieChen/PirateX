namespace PirateX.Core
{
    public interface IMessageSender
    {
        byte[] SendMessage<T>(ActorContext context, T t);

        byte[] SendMessage(ActorContext context, string msg);

        byte[] SendSeed<T>(ActorContext context, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, T t);

        void PushMessage<T>(int rid, T t);

        /// <summary>
        /// 推送给玩家
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="t"></param>
        void PushMessage<T>(PirateSession session, T t);
        /// <summary>
        /// 全服推送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="did"></param>
        /// <param name="t"></param>
        void PushMessageToDistrict<T>(int did, T t);

        //void PushMessage<T>(int rid, T t);

        //void PushMessage<T>(int rid, string name, T t);
    }

    public interface IMessagePusher
    {
        /// <summary>
        /// 推送给玩家
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="t"></param>
        void PushMessage<T>(PirateSession session, T t);
        /// <summary>
        /// 全服推送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="did"></param>
        /// <param name="t"></param>
        void PushMessageToDistrict<T>(int did, T t);
    }
}
