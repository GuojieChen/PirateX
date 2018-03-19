using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PirateX.Schedule.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = HostFactory.New(c =>
            {
                c.Service<WorkerService>(s =>
                {
                    s.ConstructUsing(name => new WorkerService(">tcp://localhost:5601", ">tcp://localhost:5602"));
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());
                });

                c.UseNLog();
            });

            host.Run();
        }
    }
}
