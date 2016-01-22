using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PirateX.Core.Broadcas;
using PirateX.Protocol;
using SuperSocket.SocketBase;

namespace PirateX
{
    public class SessionMessageBroadcast<TSession> : IMessageBroadcast, IDisposable
        where TSession : GameSession<TSession>, new()
    {
        public IAppServer<TSession> AppServer { get; private set; }

        private Queue<BroadcastMessageToRole> MessageQueue = new Queue<BroadcastMessageToRole>();

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private BroadcastWorker worker;

        ManualResetEvent waitForNotification = new ManualResetEvent(false);

        public SessionMessageBroadcast(IAppServer<TSession> appServer)
        {
            if(appServer == null)
                throw new ArgumentNullException(nameof(appServer));

            AppServer = appServer;
            worker = new BroadcastWorker(DoWork);
        }


        private void DoWork(CancellationTokenSource cancelTokenSource)
        {
            while (!cancelTokenSource.IsCancellationRequested)
            {
                if (!MessageQueue.Any())
                {
                    Thread.Sleep(200);
                    continue;
                }

                var message = MessageQueue.Dequeue();

                IEnumerable<TSession> sessions = null;
                switch (message.T)
                {
                    case MessageEnum.Role:
                        sessions = AppServer.GetSessions(item => message.Rids.Contains(item.Rid));
                        break;
                    case MessageEnum.District:
                        sessions = AppServer.GetSessions(item => message.Dids.Contains(item.ServerId));
                        break;
                }

                if (sessions != null)
                    foreach (var session in sessions)
                    {
                        session.SendMessage(new ProtocolMessage
                        {
                            B = message.TypeName,
                            D = message.Message
                        });
                    }
            }
        }

        public void Send<T>(T msg, params long[] rids)
        {
            if (cancelTokenSource.IsCancellationRequested)
                return;

            MessageQueue.Enqueue(new BroadcastMessageToRole()
            {
                Message = msg,
                T = MessageEnum.Role,
                Rids = rids,
                TypeName = typeof(T).Name
            });

        }

        public void SendToDistrict<T>(T msg, params int[] districtId)
        {
            if (cancelTokenSource.IsCancellationRequested)
                return;

            MessageQueue.Enqueue(new BroadcastMessageToRole()
            {
                Message = msg,
                T = MessageEnum.District,
                Dids = districtId,
                TypeName = typeof(T).Name
            });
        }


        internal class BroadcastWorker : IDisposable
        {
            public BroadcastWorker(Action<CancellationTokenSource> worker)
            {
                this.Id = Guid.NewGuid().ToString();
                this.CancelTokenSource = new CancellationTokenSource();
                this.WorkerTask = Task.Factory.StartNew(() => worker(this.CancelTokenSource), TaskCreationOptions.LongRunning);
            }

            public void Dispose()
            {
                CancelTokenSource.Cancel();
            }

            public string Id { get; private set; }

            public Task WorkerTask { get; private set; }

            public CancellationTokenSource CancelTokenSource { get; set; }
        }

        internal class BroadcastMessageToRole
        {
            public object Message { get; set; }

            public long[] Rids { get; set; }

            public int[] Dids { get; set; }

            public MessageEnum T { get; set; }

            public string TypeName { get; set; }
        }

        public enum MessageEnum
        {
            Role,
            District
        }

        public void Dispose()
        {
            this.cancelTokenSource.Cancel();
            worker.Dispose();
        }
    }


}
