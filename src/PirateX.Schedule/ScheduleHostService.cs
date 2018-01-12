using System;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
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
        private static PushSocket sender = new PushSocket(System.Configuration.ConfigurationManager.AppSettings["PushConnectionString"]);//"@tcp://*:5557"
        /// <summary>
        /// 上报配置和结果
        /// </summary>
        private static ResponseSocket responseSocket = new ResponseSocket(System.Configuration.ConfigurationManager.AppSettings["ResponseConnectionString"]);//"@tcp://*:5558"

        private NetMQPoller Poller = new NetMQPoller()
        {
            sender,
            responseSocket,
            NetMqQueue
        };

        public void Start()
        {
            Scheduler.Start();

            responseSocket.ReceiveReady += ProcessRequest;
            NetMqQueue.ReceiveReady += (o, args) =>
            {
                var msg = NetMqQueue.Dequeue();
                sender.SendFrame(msg);
            };

            Poller.RunAsync();

            //pull tasksconfig
            NetMqQueue.Enqueue(JsonConvert.SerializeObject(new
            {
                Cmd = "tasksconfig"
            }));

            Logger.Debug("start ok");
        }

        private void ProcessRequest(object o, NetMQSocketEventArgs e)
        {
            //接收配置
            var msgstr = responseSocket.ReceiveFrameString();
            var jsonMsg = JObject.Parse(msgstr);
            if (Logger.IsInfoEnabled) Logger.Info(msgstr);

            var result = string.Empty;
            try
            {
                switch (jsonMsg["Cmd"].ToString().ToLower())
                {
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
                        jobDetail.JobDataMap.Add("container", gameContainer2);

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
                        //result = JsonConvert.SerializeObject(new
                        //{
                        //    Cmd = "getconfig",
                        //    ShareDb = shareDb,
                        //    MongoHost = System.Configuration.ConfigurationManager.AppSettings["MongoHost"] ?? "",
                        //    MongoDatabase = System.Configuration.ConfigurationManager.AppSettings["MongoDatabase"],
                        //});
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

            sender?.Close();
            sender?.Dispose();

            responseSocket?.Close();
            responseSocket?.Dispose();

            Poller.Stop();
        }

    }
}
