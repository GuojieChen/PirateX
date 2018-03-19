using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// 上报配置和结果
        /// </summary>
        private static ResponseSocket responseSocket;

        private Proxy _proxy;
        private NetMQPoller Poller = null; 

        public static IEnumerable<ScheduleDistrictConfig> Configs = null;

        private string _frontendConnect = "inproc://host";

        private string _configString = string.Empty;

        public ScheduleHost(string backendConnect,string responseConnectionString,string configString)
        {
            _configString = configString;
            _proxy = new Proxy(new RouterSocket($"@{_frontendConnect}"),new DealerSocket(backendConnect) );
            responseSocket = new ResponseSocket(responseConnectionString);
            responseSocket.ReceiveReady += ProcessRequest;
            Poller = new NetMQPoller()
            {
                responseSocket
            };
        }


        public void Start()
        {
            Task.Factory.StartNew(_proxy.Start);
            Poller.RunAsync();
            Scheduler.Start();

            Logger.Debug("start ok");
        }

        private void ProcessRequest(object o, NetMQSocketEventArgs e)
        {
            //接收配置
            var msgstr = e.Socket.ReceiveFrameString();
            var jsonMsg = JObject.Parse(msgstr);
            if (Logger.IsInfoEnabled)
                Logger.Info(msgstr);

            var result = string.Empty;
            try
            {
                switch (jsonMsg["Cmd"].ToString().ToLower())
                {
                    case "configs":
                        Configs = jsonMsg["List"].ToObject<IEnumerable<ScheduleDistrictConfig>>();// .Children().ToList();// [].Values<>().AsEnumerable();
                        break;
                    case "schedule":
                        var identity = jsonMsg.Value<string>("Identity");
                        var name = jsonMsg.Value<string>("Name");
                        var cronschedule = jsonMsg.Value<string>("CronSchedule");

                        if (Scheduler.CheckExists(new JobKey(identity)))
                            break;

                        var jobDetail = JobBuilder.Create()
                            .StoreDurably()
                            .OfType<TaskDistributionJob>()
                            .WithIdentity(identity)
                            .Build();

                        jobDetail.JobDataMap.Add("identity", identity);
                        jobDetail.JobDataMap.Add("requestConnectionString", _frontendConnect);
                        jobDetail.JobDataMap.Add("name", name);
                        jobDetail.JobDataMap.Add("configs", Configs);

                        var trigger = TriggerBuilder.Create()
                            .WithIdentity(identity)
                            .WithCronSchedule(cronschedule)
                            .StartAt(DateTime.Now)
                            .Build();

                        Scheduler.ScheduleJob(jobDetail, trigger);

                        if (Logger.IsInfoEnabled)
                            Logger.Info($"TASK ADD Name:[{identity}] with {cronschedule}");

                        break;
                    case "getconfig": //启动
                        result = JsonConvert.SerializeObject(new
                        {
                            Cmd = "getconfig",
                            ConfigString = _configString,
                            Configs = Configs
                        });
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
