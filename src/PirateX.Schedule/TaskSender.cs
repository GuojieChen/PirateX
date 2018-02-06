using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMQ;
using Newtonsoft.Json;
using NLog;
using PirateX.Core;
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
            var configs = context.JobDetail.JobDataMap["configs"] as IEnumerable<IDistrictConfig>;

            var queue = new Queue<IDistrictConfig>(configs);//(container.GetServerConfigs().Where(s => s.CacheDb > 0));

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

                    if (Logger.IsInfoEnabled) Logger.Info($"task {name} start  --->{config}");
                }
                catch (Exception exception)
                {
                    if (Logger.IsErrorEnabled) Logger.Error(exception);
                }
            }
        }
    }
}
