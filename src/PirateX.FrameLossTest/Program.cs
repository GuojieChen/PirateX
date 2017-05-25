using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using PirateX.Client;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;

namespace PirateX.FrameLossTest
{
    class Program
    {
        private static ManualResetEvent mre;
        private static int Count = 100;

        private static readonly object _lock = new object();
        private static double span1Sum = 0.0;
        private static double span2Sum = 0.0;
        private static double span3Sum = 0.0;
        private static double span4Sum = 0.0;

        private static double spanT1Sum = 0.0;
        private static double spanT2Sum = 0.0;
        private static double spanT3Sum = 0.0;
        private static double spanT4Sum = 0.0;

        private static int spanCount = 0;
        private static int newSeedCount = 0;
        private static string host = "192.168.1.119";
        private const string TestCommand = "Test";
        static void Main(string[] args)
        {
            if (args != null && args.Length!=0)
            {
                if (args.Length >= 2)
                {
                    string c = args[1];
                    if (!string.IsNullOrEmpty(c))
                    {
                        bool result = int.TryParse(c, out Count);
                        if (result)
                            Console.WriteLine($"设置{Count}个客户端成功");
                        else
                        {
                            throw new Exception("客户端数量设置错误，应该为一个Int范围内的值");
                        }
                        Thread.Sleep(500);
                    }
                }
                if (args.Length >= 1)
                {
                    string host_str = args[0];
                    if (!string.IsNullOrEmpty(host_str))
                    {
                        host = host_str;
                    }
                }
            }
            else
            {
                Console.WriteLine($"本次测试默认{Count}个客户端");
            }
            mre = new ManualResetEvent(false);
            for (int i = 0; i < Count; i++)
            {
                var thread = new Thread(StartTest);
                thread.Start(i);
            }
            Thread.Sleep(8000);
            mre.Set();
            Console.WriteLine("mre has been set");
            while (spanCount != Count)
            {
                Thread.Sleep(2000);
                Console.WriteLine($"客户端总数量：{Count} 服务器已回复的数量:{spanCount} ");
            }
            var average1 = span1Sum / spanCount;
            var average2 = span2Sum / spanCount;
            var average3 = span3Sum / spanCount;
            var average4 = span4Sum / spanCount;

            var averageT1 = spanT1Sum / spanCount;
            var averageT2 = spanT2Sum / spanCount;
            var averageT3 = spanT3Sum / spanCount;
            var averageT4 = spanT4Sum / spanCount;

            Console.WriteLine("average tin->tout time loss:{0} ms", average1);
            Console.WriteLine("average tin->itin time loss:{0} ms", average2);
            Console.WriteLine("average itin->itout time loss:{0} ms", average3);
            Console.WriteLine("average itout->tout time loss:{0} ms", average4);

            Console.WriteLine("average itin1->itin thread start time loss:{0} ms", averageT1);
            Console.WriteLine("average itin5->itin1 OnReceive method process time loss:{0} ms", averageT2);
            Console.WriteLine("average itin6->itin5 send message time loss:{0} ms", averageT3);

            Console.WriteLine("average itin1->itin time loss:{0} ms", averageT4);
            Console.Read();
        }

        private static void StartTest(object i)
        {
            int index = (int)i;
            Console.WriteLine("loss test thread started " + index);
//            string host = "192.168.1.232";
            int port = 4012;
            int userId = 30110 + index;
            int serverId = 2;

            var tokenQuery = HttpUtility.ParseQueryString($"rid={userId}&did={serverId}");
            var token = new Token()
            {
                Did = Convert.ToInt32(tokenQuery["did"]),
                Rid = Convert.ToInt32(tokenQuery["rid"]),
                Uid = tokenQuery["uid"]
            };
            //            string token = $"rid={userId}&did={serverId}";
            var exHeaders = HttpUtility.ParseQueryString("c=" + TestCommand);
            exHeaders.Add("format", "json");

            var client = new PirateXClient($"ps://{host}:{port}", HttpUtility.UrlEncode(Convert.ToBase64String(new ProtoResponseConvert().SerializeObject(token))));
            client.DefaultFormat = "json";
            client.OnReceiveMessage += (o, args) =>
            {
                //                Console.WriteLine("matching received " + index); //Encoding.UTF8.GetString(args.Package.ContentBytes)
                lock (_lock)
                {
                    if (args.Msg == TestCommand)
                    {
                        var tin = Convert.ToInt64(args.Package.Headers["_tin_"]);

                        var tin1 = Convert.ToInt64(args.Package.Headers["_itin_1_"]);

                        var itin = Convert.ToInt64(args.Package.Headers["_itin_"]);

                        var itin1 = Convert.ToInt64(args.Package.Headers["_t1_"]);
                        var itin2 = Convert.ToInt64(args.Package.Headers["_t2_"]);
                        var itin3 = Convert.ToInt64(args.Package.Headers["_t3_"]);
                        var itin4 = Convert.ToInt64(args.Package.Headers["_t4_"]);
                        var itin5 = Convert.ToInt64(args.Package.Headers["_t5_"]);
                        var itin6 = Convert.ToInt64(args.Package.Headers["_t6_"]);
                        Console.WriteLine(itin6 + "   " + index);

                        var itout = Convert.ToInt64(args.Package.Headers["_itout_"]);
                        var tout = Convert.ToInt64(args.Package.Headers["_tout_"]);
                        var span_tin_tout = ((double)(tout - tin)) / TimeSpan.TicksPerMillisecond;
                        var span_tin_itin = ((double)(itin - tin)) / TimeSpan.TicksPerMillisecond;
                        var span_itin_itout = ((double)(itout - itin)) / TimeSpan.TicksPerMillisecond;
                        var span_itout_tout = ((double)(tout - itout)) / TimeSpan.TicksPerMillisecond;

                        var span_task_start = ((double)(itin1 - itin)) / TimeSpan.TicksPerMillisecond;
                        var span_OnReceive_process = ((double)(itin5 - itin1)) / TimeSpan.TicksPerMillisecond;
                        var span_send_message = ((double)(itin6 - itin5)) / TimeSpan.TicksPerMillisecond;
                        
                        var span_tin1 = ((double)(itin - tin1)) / TimeSpan.TicksPerMillisecond;
                        //                        Console.WriteLine($"{index} span:{span_tin_tout} ms");

                        spanCount++;

                        span1Sum += span_tin_tout;
                        span2Sum += span_tin_itin;
                        span3Sum += span_itin_itout;
                        span4Sum += span_itout_tout;

                        spanT1Sum += span_task_start;
                        spanT2Sum += span_OnReceive_process;
                        spanT3Sum += span_send_message;

                        spanT4Sum += span_tin1;
                    }
                    else
                    {
                        //args.Msg = "NewSeed"
                        newSeedCount++;
                    }
                }
            };
            try
            {
                client.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                return;
            }
            //            Console.WriteLine("connnect success " + index);
            mre.WaitOne();
            //            Console.WriteLine("send match command " + index);
            client.Send("RoleInfo", "pattern=2&name=dst" + index, exHeaders);
            //            Thread.Sleep(1000);
            //            Console.WriteLine("send complete " + index);
        }
    }
}
