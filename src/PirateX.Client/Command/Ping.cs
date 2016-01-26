namespace PirateX.Client.Command
{
    public class Ping : JsonExecutorBase<PSocketClient,PingResponse>
    {
        public override void Excute(PSocketClient pSocket, PingResponse data)
        {
            
        }
    }

    public class PingResponse
    {
        
    }
}
