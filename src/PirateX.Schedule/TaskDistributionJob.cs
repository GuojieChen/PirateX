using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using NLog;
using PirateX.Core;
using Quartz;

namespace PirateX.Schedule
{
    /// <summary>
    /// 任务分发
    /// </summary>
    public class TaskDistributionJob : IJob
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public void Execute(IJobExecutionContext context)
        {
            var requestConnectionString = context.JobDetail.JobDataMap["requestConnectionString"] as string;
            var identity = context.JobDetail.JobDataMap["identity"] as string;
            var name = context.JobDetail.JobDataMap["name"] as string;
            var queue = new Queue<ScheduleHost.ScheduleDistrictConfig>(ScheduleHost.Configs);//(container.GetServerConfigs().Where(s => s.CacheDb > 0));

            while (queue.Any())
            {
                //这里按照连接的机器数量做分发控制
                var config = queue.Dequeue();

                if (Logger.IsInfoEnabled)
                    Logger.Info($"TASK START Name:[{name}] TO [{config.Id}]");

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (var r = new RequestSocket($">{requestConnectionString}"))
                        {
                            if (r.TrySendFrame(JsonConvert.SerializeObject(new
                            {
                                Name = name,
                                Config = config,
                            })))
                            {
                                if (r.TryReceiveFrameString(TimeSpan.FromSeconds(30), out var repMsg))
                                {
                                    if (Logger.IsInfoEnabled)
                                        Logger.Info($"TASK DONE Name:[{name}] {repMsg}");
                                }
                                else
                                {
                                    Logger.Warn($"TASK RECEIVE TIMEOUT Name:[{name}] TO [{config.Id}]");
                                }
                            }
                            else
                            {
                                Logger.Warn($"TASK SEND TIMEOUT Name:[{name}] TO [{config.Id}]");
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        if (Logger.IsErrorEnabled)
                            Logger.Error(exception);
                    }
                });
            }
        }
    }
}
