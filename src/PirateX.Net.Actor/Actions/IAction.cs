using NetMQ;
using PirateX.Protocol.Package;
using Topshelf.Logging;

namespace PirateX.Net.Actor.Actions
{
    public interface IAction
    {
        IProtocolPackage ProtocolPackage { get; set; }
        LogWriter Logger { get; set; }

        /// <summary>
        /// 动作名称
        /// </summary>
        string Name { get; set; }

        ActorContext Context { get; set; }

        NetMQQueue<NetMQMessage> MessageQeue { get; set; }
        void Execute();
    }
}
