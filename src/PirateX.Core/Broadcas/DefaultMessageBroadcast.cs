namespace PirateX.Core
{
    public class DefaultMessageBroadcast : IMessageBroadcast
    {
        public void Send<T>(T msg, params long[] rids)
        {
        }

        public void SendToDistrict<T>(T msg, params int[] districtId)
        {

        }
    }
}
