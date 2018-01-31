using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using PirateX.Core;
using ProtoBuf;

namespace PirateX.ProtobufInitialize
{
    /// <summary>
    /// proto协议描述同步服务
    /// 
    /// </summary>
    public class ProtobufService : IProtoService
    {
        public Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// proto文件存放目录
        /// </summary>
        private static readonly string ProtoDir = Path.Combine(global::System.AppDomain.CurrentDomain.BaseDirectory, "protos");

        private static readonly string ProtoHashFile = Path.Combine(ProtoDir, "protoshash.txt");

        private static readonly string ProtoFile = Path.Combine(ProtoDir, "model.proto");

        private static string _currentModuleVersionId = string.Empty;

        private static byte[] _proto = null; 

        private bool _isInitOk = false;

        public void Init(List<Assembly> list)
        {
            if(Logger.IsTraceEnabled)
                Logger.Trace("---------- ProtobufService.Init BEGIN----------");

            //list.ForEach(Init);
            var builder = new StringBuilder();
            list.ForEach(item=>builder.Append(GetProtos(item)));

            if (Directory.Exists(ProtoDir))
                Directory.Delete(ProtoDir,true);

            var exception = false; 
            var proto = CheckRemove(builder.ToString(),out exception);

            Directory.CreateDirectory(ProtoDir);
            File.Create (ProtoFile).Close();
            File.Create(ProtoHashFile).Close();
            File.AppendAllText(ProtoFile, proto);

            if(exception)
                throw new FileLoadException("加载中出现异常，请查看protos\\model.proto中的描述");

            #region Generate .bin file
            var proc = new Process { StartInfo = { WorkingDirectory = ProtoDir, FileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\protoc.exe" ,Arguments = "--descriptor_set_out=model.bin --include_imports model.proto" } };
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.EnableRaisingEvents = true;
            //proc.OutputDataReceived += (sender, args) =>
            //{
            //    Console.WriteLine(args.Data);
            //};
            proc.Start();
            
            if(Logger.IsTraceEnabled)
                Logger.Trace(proc.StandardOutput.ReadToEnd());

            proc.WaitForExit();
            _proto = File.ReadAllBytes(Path.Combine(ProtoDir, "model.bin"));

            _currentModuleVersionId = _proto.GetMD5();
            File.AppendAllText(ProtoHashFile, _currentModuleVersionId);
            #endregion

            if (Logger.IsTraceEnabled)
                Logger.Trace("---------- ProtobufService.Init END----------");
        }

        private ISet<string> messageHash = new HashSet<string>();

        private string CheckRemove(string pbcontext, out bool exception)
        {
            exception = false;

            var lines = pbcontext.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            var builder = new StringBuilder();

            StringBuilder tempbuilder = null; 
            foreach (var line in lines)
            {
                if(line.StartsWith("package"))
                    continue;

                if (line.Contains("import \"bcl.proto\""))
                {
                    exception = true;
                    builder.AppendLine("不支持DateTime字段，请用时间戳（long或者int）来替换");
                }

                if (line.EndsWith("{"))
                    tempbuilder = new StringBuilder();
                else if (line.StartsWith("}") && tempbuilder != null)
                {
                    tempbuilder.AppendLine(line);

                    var message = tempbuilder.ToString();
                    var hash = message.GetMD5();
                    if (!messageHash.Contains(hash))
                    {
                        builder.AppendLine(message);
                        messageHash.Add(hash);
                    }
                    tempbuilder = null;
                    continue;
                }

                tempbuilder?.AppendLine(line);
            }

            return builder.ToString();
        }

        public string GetProtos(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(item => item.GetCustomAttribute<ProtoContractAttribute>()!=null);
            
            var stringBuilder = new StringBuilder();
            foreach (var type in types)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                var proto = typeof(Serializer).GetMethod("GetProto",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(type)
                    .Invoke(this, null);
                
                PrepareSerializer(type);
                stringBuilder.Append(proto);

                if (Logger.IsTraceEnabled)
                    Logger.Trace($"GetProtos->{type.Name}");
            }


            return stringBuilder.ToString();
        }

        private void PrepareSerializer(Type type)
        {
            typeof(Serializer).GetMethod("PrepareSerializer",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(type)
                .Invoke(this, null);
        }

        public string GetProtosHash()
        {
            return _currentModuleVersionId;
        }

        public byte[] GetProto()
        {
            return _proto;
        }
    }
}
