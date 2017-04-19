using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace PirateX.Net
{
    public class ActorConfig
    {
        public string PullSocketString { get; set; }

        public string PushsocketString { get; set; }
    }


    public interface IActorNetService
    {
        void SetUp();
        void Start();
        void Stop();
    }

    public abstract class ActorNetService : IActorNetService
    {
        private ActorConfig config;

        private PullSocket PullSocket { get; set; }
        private PushSocket PushSocket { get; set; }

        private NetMQPoller Poller { get; set; }
        private readonly NetMQQueue<NetMQMessage> MessageQueue = new NetMQQueue<NetMQMessage>();

        public ActorNetService(ActorConfig config)
        {
            this.config = config;
        }

        public virtual void SetUp()
        {
            PullSocket = new PullSocket(config.PullSocketString);
            PullSocket.ReceiveReady += ProcessTaskPullSocket;

            PushSocket = new PushSocket(config.PushsocketString);
            Poller = new NetMQPoller() { PullSocket, PushSocket, MessageQueue };

            MessageQueue.ReceiveReady += (sender, args) => { PushSocket.SendMultipartMessage(args.Queue.Dequeue()); };
        }

        protected abstract void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e);

        protected void EnqueueMessage(NetMQMessage message)
        {
            MessageQueue.Enqueue(message);
        }

        public virtual void Start()
        {
            Poller.RunAsync();
        }

        public virtual void Stop()
        {
            Poller?.Stop();
            PullSocket?.Close();
        }

    }
}
