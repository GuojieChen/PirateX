using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PirateX.Core.Domain.Entity;
using ProtoBuf;
using Topshelf.Logging;

namespace PirateX.Net.Actor.ProtoSync
{
    /// <summary>
    /// proto协议描述同步服务
    /// 
    /// </summary>
    public class ProtobufService : IProtoService
    {
        public LogWriter Logger = HostLogger.Get(typeof(ProtobufService));

        /// <summary>
        /// proto文件存放目录
        /// </summary>
        private static readonly string ProtoDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "protos");

        private static readonly string ModuleVersionIdFileName = Path.Combine(ProtoDir, "ModuleVersionId.txt");

        private static readonly string ProtoHashFile = Path.Combine(ProtoDir, "protoshash.json");

        private static string _currentModuleVersionId = string.Empty;

        private static IDictionary<string, string> _protoshash = new Dictionary<string, string>();

        private bool _isInitOk = false;
        public void Init(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(item => typeof(IEntity).IsAssignableFrom(item));

            if (!Directory.Exists(ProtoDir))
                Directory.CreateDirectory(ProtoDir);

            if (File.Exists(ModuleVersionIdFileName))
                _currentModuleVersionId = File.ReadAllText(ModuleVersionIdFileName);
            else
                using (var f = File.Create(ModuleVersionIdFileName))
                {

                }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"previous ModuleVersionId is {_currentModuleVersionId}");

            if (File.Exists(ProtoHashFile))
            {
                var protoshash = ConvertToDic(File.ReadAllText(ProtoHashFile));
                if (protoshash != null)
                    _protoshash = protoshash;
            }
            else
            {
                using (var f = File.Create(ProtoHashFile))
                {
                }
            }

            if (!Equals(assembly.ManifestModule.ModuleVersionId.ToString(), _currentModuleVersionId))
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug("genterating proto files...");

                foreach (var type in types)
                {
                    var guid = type.GUID.ToString();

                    if (_protoshash.ContainsKey(type.Name) && Equals(_protoshash[type.Name], guid))
                        continue;

                    var proto = typeof(Serializer).GetMethod("GetProto",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                        .MakeGenericMethod(type)
                        .Invoke(this, null);

                    var protofile = Path.Combine(ProtoDir, $"{type.Name}.proto");
                    if (!File.Exists(protofile))
                    {
                        using (var f = File.Create(protofile))
                        {
                        }
                    }

                    File.WriteAllText(protofile, proto.ToString());

                    if (_protoshash.ContainsKey(type.Name))
                        _protoshash[type.Name] = guid;
                    else
                        _protoshash.Add(type.Name, guid);
                }

                _currentModuleVersionId = assembly.ManifestModule.ModuleVersionId.ToString();
                File.WriteAllText(ModuleVersionIdFileName, assembly.ManifestModule.ModuleVersionId.ToString());
                File.WriteAllText(ProtoHashFile, ConvertToString(_protoshash));
            }

            _isInitOk = true;
        }


        private static string ConvertToString(IDictionary<string, string> dic)
        {
            var stringBuilder = new StringBuilder();
            foreach (var kv in dic)
            {
                stringBuilder.Append(kv.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(kv.Value);
                stringBuilder.Append("&");
            }

            return stringBuilder.ToString().TrimEnd('&');
        }

        private static IDictionary<string, string> ConvertToDic(string str)
        {
            var dic = new Dictionary<string,string>();

            var ss = str.Trim(' ').Split(new char[] {'&'});
            foreach (var s in ss)
            {
                var item = s.Split(new char[] {'='});
                var key = item[0];
                var value = item[1];

                if (dic.ContainsKey(key))
                    dic[key] = value; 
                else 
                    dic.Add(key,value);
            }

            return dic;
        } 

        public string GetProtosHash()
        {
            return _currentModuleVersionId;
        }

        public IDictionary<string, string> GetProtosHashDic(Assembly assembly)
        {
            return _protoshash;
        }
    }
}
