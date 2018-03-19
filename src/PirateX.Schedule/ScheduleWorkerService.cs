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
using PirateX.Core;

namespace PirateX.Schedule
{
    public abstract class ScheduleWorkerService<TWorkerService>
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IDictionary<string, Type> TaskDic = new Dictionary<string, Type>();

        private static CancellationTokenSource ct = new CancellationTokenSource();

        private static NetMQPoller poller = null;

        private static JObject ConfigJson = null;

        private IDistrictContainer _districtContainer;


        private string _frontendConnect;
        private string _requestConnectionString;
        public ScheduleWorkerService(string frontendConnect, string requestConnectionString)
        {
            _frontendConnect = frontendConnect;
            _requestConnectionString = requestConnectionString;
        }

        public void Start()
        {
            var jsonStr = Req(new { Cmd = "getconfig"},10);

            var json = JObject.Parse(jsonStr);
            ConfigJson = json;
            if (Logger.IsInfoEnabled)
                Logger.Info(jsonStr);

            _districtContainer = GetDistrictContainer(json);
            _districtContainer.InitContainers(new ContainerBuilder());

            if (!json["Configs"].HasValues)
            {
                Req(new
                {
                    Cmd = "configs",
                    List = _districtContainer.GetDistrictConfigs()
                });

                AddJobs();
            }


            poller = new NetMQPoller();

            for (int i = 0; i < 2; i++)
                Task.Factory.StartNew(WorkerTask, _frontendConnect, _c_token.Token);

            poller.RunAsync();
        }

        private CancellationTokenSource _c_token = new CancellationTokenSource();
        private void WorkerTask(object connectTo)
        {
            using (var server = new ResponseSocket(Convert.ToString(connectTo)))
            {
                while (!_c_token.Token.IsCancellationRequested)
                {
                    ThreadProcessRequest(server);
                }
            }
        }

        private void ThreadProcessRequest(NetMQSocket socket)
        {
            string response = string.Empty;
            bool receiveok = false; 
            try
            {
                if (socket.TryReceiveFrameString(out var msg))
                {
                    receiveok = true;
                    var json = JObject.Parse(msg);
                    if (Logger.IsDebugEnabled)
                        Logger.Debug(
                            $"Thread:{Thread.CurrentThread.ManagedThreadId}:{Thread.CurrentThread.IsThreadPoolThread}->{msg}");

                    var name = json.Value<string>("Name");
                    var serverconfig = json["Config"].ToObject<ScheduleHost.ScheduleDistrictConfig>();

                    if (!TaskDic.ContainsKey(name))
                        return;
                    var type = TaskDic[name];
                    if (!(Activator.CreateInstance(type) is IGameJobTask task))
                        return;
                    task.ConfigJson = ConfigJson;

                    try
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        task.Execute(_districtContainer, serverconfig);
                        sw.Stop();

                        response = JsonConvert.SerializeObject(new
                        {
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

                        response = JsonConvert.SerializeObject(new
                        {
                            Host = Dns.GetHostName(),
                            Name = name,
                            Id = serverconfig.Id,
                            Msg = "error",
                            Error = ex
                        });
                    }
                }
                else
                    return;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
            finally
            {
                if(receiveok)
                    socket.SendFrame(response);
            }
        }

        protected abstract IDistrictContainer GetDistrictContainer(JObject jObject);

        private void ProcessTask(object sender, NetMQSocketEventArgs e)
        {
            try
            {
                var msg = e.Socket.ReceiveFrameString();

                var json = JObject.Parse(msg);
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"thread:{Thread.CurrentThread.ManagedThreadId}:{Thread.CurrentThread.IsThreadPoolThread}->{msg}");
                var cmd = json.Value<string>("Cmd");

                switch (cmd)
                {
                    case "task":
                        
                        break;
                    case "hoststart"://上报任务
                        // 上报配置和任务信息
                        Req(new
                        {
                            Cmd = "configs",
                            List = _districtContainer.GetDistrictConfigs()
                        });

                        AddJobs();
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private string Req(object msg,int trycount = 0)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"SEND->{JsonConvert.SerializeObject(msg)}");

            using (var r = new RequestSocket(_requestConnectionString))
            {
                if (r.TrySendFrame(JsonConvert.SerializeObject(msg)))
                {
                    if (r.TryReceiveFrameString(TimeSpan.FromSeconds(5), out var repMsg))
                    {
                        return string.IsNullOrEmpty(repMsg) ? string.Empty : repMsg;
                    }
                }

                if (trycount > 0)
                {
                    Thread.Sleep(1000);
                    return Req(msg, trycount--);
                }
            }

            return string.Empty;
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
            poller.Stop();
        }

    }
}
