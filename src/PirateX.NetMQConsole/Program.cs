using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace PirateX.NetMQConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ActorService>((s =>
                {
                    s.ConstructUsing(name => new WorkerService(new WorkerConfig()
                    {
                        PullConnectHost = ">tcp://localhost:5556"
                    }, new TestContainer()));
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());
                }));

                x.RunAsLocalSystem();

            });

        }
    }
}
