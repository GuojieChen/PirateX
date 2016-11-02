using NetMQ;
using PirateX.Protocol.Package;
using Topshelf.Logging;

namespace PirateX.Net.Actor.Actions
{
    public abstract class ActionBase:IAction
    {
        public IProtocolPackage ProtocolPackage { get; set; }
        public LogWriter Logger { get; set; }
        public string Name { get; set; }
        public ActorContext Context { get; set; }
        public abstract void Execute();

        public NetMQQueue<NetMQMessage> MessageQeue { get; set; }
    }
}
