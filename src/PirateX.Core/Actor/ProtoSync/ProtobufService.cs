using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Utils;
using ProtoBuf;

namespace PirateX.Core.Actor.ProtoSync
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

            var proto = CheckRemove(builder.ToString());

            Directory.CreateDirectory(ProtoDir);
            File.Create (ProtoFile).Close();
            File.Create(ProtoHashFile).Close();

            File.AppendAllText(ProtoFile, proto);

            #region Generate .bin file

            //var batFile = Path.Combine(ProtoDir, "run.bat");
            //var fs1 = new FileStream(batFile, FileMode.Create, FileAccess.Write);
            //var sw = new StreamWriter(fs1);
            //sw.WriteLine("..\\protoc --descriptor_set_out=model.bin --include_imports model.proto");
            //sw.Close();
            //fs1.Close();

            var proc = new Process { StartInfo = { WorkingDirectory = ProtoDir, FileName = "..\\protoc",Arguments = "--descriptor_set_out=model.bin --include_imports model.proto"} };
            proc.Start();
            proc.WaitForExit();

            _proto = File.ReadAllBytes(Path.Combine(ProtoDir, "model.bin"));

            _currentModuleVersionId = _proto.GetMD5();
            File.AppendAllText(ProtoHashFile, _currentModuleVersionId);
            #endregion

            if (Logger.IsTraceEnabled)
                Logger.Trace("---------- ProtobufService.Init END----------");
        }

        private ISet<string> messageHash = new HashSet<string>();

        private string CheckRemove(string pbcontext)
        {
            var lines = pbcontext.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            var builder = new StringBuilder();

            StringBuilder tempbuilder = null; 
            foreach (var line in lines)
            {
                if(line.StartsWith("package"))
                    continue;

                //if(line.Contains("import bc1.proto"))

                if(line.StartsWith("message"))
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

                stringBuilder.Append(proto);
            }

            return stringBuilder.ToString();
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
