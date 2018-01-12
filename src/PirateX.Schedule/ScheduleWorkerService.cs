using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using PirateX.Core.Container;

namespace PirateX.Schedule
{
    public class ScheduleWorkerService<TWorkerService>
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IDictionary<string, Type> TaskDic = new Dictionary<string, Type>();

        private static CancellationTokenSource ct = new CancellationTokenSource();
        /// <summary>
        /// 拉取任务
        /// </summary>
        private static PullSocket tasks = new PullSocket(System.Configuration.ConfigurationManager.AppSettings.Get("PullConnectionString"));

        private static NetMQPoller poller = new NetMQPoller() { tasks };

        private static JObject ConfigJson = null;

        private IDistrictContainer _districtContainer;

        public void Start()
        {
            var jsonStr = Req(new { Cmd = "getconfig" });
            var json = JObject.Parse(jsonStr);
            ConfigJson = json;
            if (Logger.IsInfoEnabled) Logger.Info(jsonStr);

            //gameContainer2 = new GameContainer2(this, json.Value<string>("ShareDb"));
            //gameContainer2.SetUp2(true);
            //gameContainer2.Load();

            _districtContainer.InitContainers(new ContainerBuilder());

            AddJobs();
            tasks.ReceiveReady += ProcessTask;

            poller.RunAsync();
        }

        private void ProcessTask(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveFrameString();

            var json = JObject.Parse(msg);
            if (Logger.IsDebugEnabled)
                Logger.Debug($"thread:{Thread.CurrentThread.ManagedThreadId}:{Thread.CurrentThread.IsThreadPoolThread}->{msg}");
            var cmd = json.Value<string>("Cmd");

            switch (cmd)
            {
                case "task":
                    var name = json.Value<string>("Name");
                    var serverconfig = json.Value<IDistrictConfig>("Config");
                    var culture = json.Value<string>("Culture");

                    if (!TaskDic.ContainsKey(name))
                        return;
                    var type = TaskDic[name];
                    if (!(Activator.CreateInstance(type) is IGameJobTask task))
                        return;
                    task.ConfigJson = ConfigJson;

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(culture))
                            {
                                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
                                Thread.CurrentThread.CurrentUICulture.DateTimeFormat = new CultureInfo("zh-CN", false).DateTimeFormat;
                            }

                            Req(new
                            {
                                Cmd = "taskinfo",
                                Host = Dns.GetHostName(),
                                Name = name,
                                Msg = "start",
                                Id = serverconfig.Id
                            });
                            var sw = new Stopwatch();
                            sw.Start();
                            task.Execute(_districtContainer, serverconfig);
                            sw.Stop();
                            Req(new
                            {
                                Cmd = "taskinfo",
                                Host = Dns.GetHostName(),
                                Name = name,
                                Msg = "done",
                                Id = serverconfig.Id,
                                sw.ElapsedMilliseconds
                            });
                        }
                        catch (Exception ex)
                        {
                            if (Logger.IsErrorEnabled) Logger.Error(ex);

                            Req(new
                            {
                                Cmd = "taskinfo",
                                Host = Dns.GetHostName(),
                                Name = name,
                                Id = serverconfig.Id,
                                Msg = "error",
                                Error = ex
                            });

                        }
                    });

                    break;
                case "tasksconfig"://上报任务
                    AddJobs();
                    break;
            }
        }

        private static string Req(object msg)
        {
            if (Logger.IsDebugEnabled) Logger.Debug(JsonConvert.SerializeObject(msg));

            using (var r = new RequestSocket(System.Configuration.ConfigurationManager.AppSettings.Get("RequestConnectionString")))
            {
                r.SendFrame(JsonConvert.SerializeObject(msg));

                string repMsg = r.ReceiveFrameString();

                if (string.IsNullOrEmpty(repMsg))
                    return string.Empty;

                return repMsg;
            }
        }

        private void AddJobs()
        {
            var types = typeof(TWorkerService).Assembly.GetTypes().Where(item => typeof(IGameJobTask).IsAssignableFrom(item));

            foreach (var type in types)
            {
                if (!TaskDic.ContainsKey(type.Name))
                    TaskDic.Add(type.Name, type);

                var atts = type.GetCustomAttributes(typeof(CronScheduleAttribute), false);
                if (atts.Any())
                {
                    for (int i = 0; i < atts.Count(); i++)
                    {
                        var identity = type.Name;
                        if (i > 0)
                            identity = $"{identity}_{(i + 1)}";

                        /*
                         * msg 结构
                         * msg[0] schedule
                         * msg[1] name
                         * msg[2] cronschedule
                         */

                        var attr = (CronScheduleAttribute)atts[i];
                        //注册任务
                        Req(new
                        {
                            Cmd = "schedule",
                            Identity = identity,
                            Name = type.Name,
                            CronSchedule = attr.CronSchedule
                        });
                    }
                }
                else
                {
                    if (Logger.IsWarnEnabled)
                        Logger.Warn($"{type.Name,-30}\tCronSchedule NULL");
                }
            } 
        }
        public void Stop()
        {
            ct.Cancel();
            tasks.Close();
            poller.Stop();
        }

    }
}
