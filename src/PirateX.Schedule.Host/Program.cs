using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PirateX.Schedule.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = HostFactory.New(c =>
            {
                c.Service<ScheduleHost>(s =>
                {
                    s.ConstructUsing(name => new ScheduleHost(
                        ConfigurationManager.AppSettings["BackendConnectionString"]
                        , ConfigurationManager.AppSettings["ResponseConnectionString"],
                        ConfigurationManager.AppSettings["ConfigString"]));
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());
                });

                c.SetServiceName(ConfigurationManager.AppSettings["ServiceName"]);
                c.UseNLog();
            });

            host.Run();
        }
    }
}
