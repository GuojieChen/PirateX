using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using PirateX.Core;
using Quartz;
using Quartz.Impl;

namespace PirateX.Schedule
{
    public class ScheduleHost
    {
        private static readonly IScheduler Scheduler = new StdSchedulerFactory().GetScheduler();
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static NetMQQueue<string> NetMqQueue = new NetMQQueue<string>();

        /// <summary>
        /// 下发任务
        /// </summary>
        private static PushSocket pushSocket = null;

        /// <summary>
        /// 上报配置和结果
        /// </summary>
        private static ResponseSocket responseSocket;

        private IEnumerable<IDistrictConfig> Configs = null;

        public ScheduleHost(string pushConnectionString,string responseConnectionString)
        {
            pushSocket = new PushSocket(pushConnectionString);

            responseSocket = new ResponseSocket(responseConnectionString);

            Poller = new NetMQPoller()
            {
                pushSocket,
                responseSocket,
                NetMqQueue
            };
        }

        private NetMQPoller Poller;

        public void Start()
        {
            Scheduler.Start();

            responseSocket.ReceiveReady += ProcessRequest;
            NetMqQueue.ReceiveReady += (o, args) =>
            {
                var msg = NetMqQueue.Dequeue();
                pushSocket.SendFrame(msg);
            };

            Poller.RunAsync();

            //pull tasksconfig
            NetMqQueue.Enqueue(JsonConvert.SerializeObject(new
            {
                Cmd = "hoststart"
            }));

            Logger.Debug("start ok");
        }

        private void ProcessRequest(object o, NetMQSocketEventArgs e)
        {
            //接收配置
            var msgstr = responseSocket.ReceiveFrameString();
            var jsonMsg = JObject.Parse(msgstr);
            if (Logger.IsInfoEnabled)
                Logger.Info(msgstr);

            var result = string.Empty;
            try
            {
                switch (jsonMsg["Cmd"].ToString().ToLower())
                {
                    case "configs":
                        Configs = jsonMsg["List"].Values<ScheduleDistrictConfig>();
                        break;

                    case "schedule":
                        var identity = jsonMsg.Value<string>("Identity");
                        var name = jsonMsg.Value<string>("Name");
                        var cronschedule = jsonMsg.Value<string>("CronSchedule");

                        if (Scheduler.CheckExists(new JobKey(identity)))
                            break;

                        var jobDetail = JobBuilder.Create()
                            .StoreDurably()
                            .OfType<TaskSender>()
                            .WithIdentity(identity)
                            .Build();

                        jobDetail.JobDataMap.Add("identity", identity);
                        jobDetail.JobDataMap.Add("queue", NetMqQueue);
                        jobDetail.JobDataMap.Add("name", name);
                        jobDetail.JobDataMap.Add("configs", Configs);

                        var trigger = TriggerBuilder.Create()
                            .WithIdentity(identity)
                            .WithCronSchedule(cronschedule)
                            .StartAt(DateTime.Now)
                            .Build();

                        Scheduler.ScheduleJob(jobDetail, trigger);

                        if (Logger.IsInfoEnabled)
                            Logger.Info($"add task {identity} with {cronschedule}");

                        break;
                    case "getconfig": //启动
                        result = JsonConvert.SerializeObject(new
                        {
                            Cmd = "getconfig",
                            ConfigUrl = "http://192.168.1.54/serverconfig.json",
                        });
                        break;
                    case "taskinfo":

                        break;

                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
            finally
            {
                responseSocket.SendFrame(result);
            }
        }


        public void Stop()
        {
            Scheduler.Shutdown();

            pushSocket?.Close();
            pushSocket?.Dispose();

            responseSocket?.Close();
            responseSocket?.Dispose();

            Poller.Stop();
        }


        public class ScheduleDistrictConfig : IDistrictConfig
        {
            public int Id { get; set; }
            public string SecretKey { get; set; }
            public int TargetId { get; set; }
        }
    }
}
