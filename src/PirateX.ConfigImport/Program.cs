using System.Web;
using CommandLine;
using System.IO;

namespace PirateX.ConfigImport
{
    class ApplicationArgs
    {
        [Option('i', "input", Required = true, HelpText = "配置文件目录")]
        public string Input { get; set; }

        [Option('w', "maxworkers", Required = false, HelpText = "工作队列，默认5个", DefaultValue = 5)]
        public int MaxWorkers { get; set; }

        [Option('e', "ignore", Required = true, HelpText = "忽略的字段" ,DefaultValue = "")]
        public string Ignore { get; set; }

        [Option('s', "connectionstring", Required = true, HelpText = "数据库连接字符串")]
        public string ConnectionString { get; set; }

        [Option('d', "dlldir", Required = true, HelpText = "模块目录")]
        public string DllDir { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var applicationArgs = new ApplicationArgs();

            Parser.Default.ParseArguments(args, applicationArgs);

            var service = new ImportService(applicationArgs.Input, applicationArgs.MaxWorkers, HttpUtility.ParseQueryString(applicationArgs.Ignore), applicationArgs.ConnectionString);

            if(File.Exists(applicationArgs.DllDir))
            {//文件
                service.Start(applicationArgs.DllDir);
            }
            else
            {//目录
                service.Start(Directory.GetFiles(applicationArgs.DllDir));
            }
            service.Stop();
        }
    }
}
