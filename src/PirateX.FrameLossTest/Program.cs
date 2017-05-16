﻿using System;
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
        private static int spanCount = 0;
        private static int newSeedCount = 0;

        private const string TestCommand = "Test";
        static void Main(string[] args)
        {
            string c = args[0];
            if (!string.IsNullOrEmpty(c))
            {
                bool result = int.TryParse(c, out Count);
                if(result)
                    Console.WriteLine($"设置{Count}个客户端成功");
                else
                {
                    Console.WriteLine($"本次测试默认{Count}个客户端");
                }
                Thread.Sleep(500);
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
            Console.WriteLine("average tin->tout time loss:{0} ms", average1);
            Console.WriteLine("average tin->itin time loss:{0} ms", average2);
            Console.WriteLine("average itin->itout time loss:{0} ms", average3);
            Console.WriteLine("average itout->tout time loss:{0} ms", average4);
            Console.Read();
        }

        private static void StartTest(object i)
        {
            int index = (int)i;
            Console.WriteLine("loss test thread started " + index);
            string host = "192.168.1.119";
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
                        var itin = Convert.ToInt64(args.Package.Headers["_itin_"]);
                        var itout = Convert.ToInt64(args.Package.Headers["_itout_"]);
                        var tout = Convert.ToInt64(args.Package.Headers["_tout_"]);
                        var span_tin_tout = ((double)(tout - tin)) / TimeSpan.TicksPerMillisecond;
                        var span_tin_itin = ((double)(itin - tin)) / TimeSpan.TicksPerMillisecond;
                        var span_itin_itout = ((double)(itout - itin)) / TimeSpan.TicksPerMillisecond;
                        var span_itout_tout = ((double)(tout - itout)) / TimeSpan.TicksPerMillisecond;
                        Console.WriteLine($"{index} span:{span_tin_tout} ms");

                        spanCount++;

                        span1Sum += span_tin_tout;
                        span2Sum += span_tin_itin;
                        span3Sum += span_itin_itout;
                        span4Sum += span_itout_tout;

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
