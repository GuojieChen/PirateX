namespace PirateX.Client.Command
{
    public class Ping : JsonExecutorBase<PirateXClient,PingResponse>
    {
        public override void Excute(PirateXClient pSocket, PingResponse data)
        {
            
        }
    }

    public class PingResponse
    {
        
    }
}
