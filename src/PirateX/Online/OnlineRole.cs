namespace PirateX.Online
{
    public class OnlineRole  :IOnlineRole
    {
        public long Id { get; set; }
        public int Did { get; set; }
        public string DistrictName { get; set; }
        public string SessionID { get; set; }
    }
}
