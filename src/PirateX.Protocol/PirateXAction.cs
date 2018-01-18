namespace PirateX.Protocol
{
    public enum PirateXAction : byte
    {
        Ping = 0,
        Req = 1,
        Closed = 2,
        Seed = 3,    //种子交换
        Push = 4,
    }
}
