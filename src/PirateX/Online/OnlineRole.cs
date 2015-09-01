namespace PirateX.Online
{
    public class OnlineRole  :IOnlineRole
    {
        public long Id { get; set; }
        public int ServerId { get; set; }
        public string ServerName { get; set; }
        public string SessionID { get; set; }
    }
}
