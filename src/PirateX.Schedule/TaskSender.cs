using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMQ;
using Newtonsoft.Json;
using NLog;
using PirateX.Core.Container;
using Quartz;

namespace PirateX.Schedule
{
    public class TaskSender : IJob
    {

        private static Logger Logger = LogManager.GetCurrentClassLogger();


        public void Execute(IJobExecutionContext context)
        {
            if (!(context.JobDetail.JobDataMap["queue"] is NetMQQueue<string> netmqqueue))
                return;

            var identity = context.JobDetail.JobDataMap["identity"] as string;
            var name = context.JobDetail.JobDataMap["name"] as string;

            //var container = context.JobDetail.JobDataMap["container"] as GameContainer2;
            //if (container == null)
            //    return;

            var queue = new Queue<IDistrictConfig>();//(container.GetServerConfigs().Where(s => s.CacheDb > 0));

            while (queue.Any())
            {
                //这里按照连接的机器数量做分发控制
                var config = queue.Dequeue();
                try
                {
                    netmqqueue.Enqueue(JsonConvert.SerializeObject(new
                    {
                        Cmd = "task",
                        Name = name,
                        Config = config,
                    }));

                    //pushsocket.SendFrame(JsonConvert.SerializeObject(new
                    //{
                    //    Cmd = "task",
                    //    Name = name,
                    //    Config = config,
                    //    Culture = Culture,
                    //}));
                    if (Logger.IsInfoEnabled) Logger.Info($"task {name} {config.Id} start");
                }
                catch (Exception exception)
                {
                    if (Logger.IsErrorEnabled) Logger.Error(exception);
                }
            }
        }
    }
}
