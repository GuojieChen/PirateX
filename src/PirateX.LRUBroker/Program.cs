using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PirateX.LRUBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            var frontRouterString = string.Empty;
            var backRouterString = string.Empty;
            var host = HostFactory.New(c =>
            {
                c.Service<LRUBrokerService>(s =>
                {
                    s.ConstructUsing(name => new LRUBrokerService(frontRouterString, backRouterString));
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());
                });

                c.AddCommandLineDefinition("FrontRouter",item=>frontRouterString = item);
                c.AddCommandLineDefinition("BackRouter", item => backRouterString = item);
            });

            host.Run();
        }
    }
}
