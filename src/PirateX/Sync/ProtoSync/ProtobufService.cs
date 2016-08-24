using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NLog;
using PirateX.Core.Domain.Entity;
using ProtoBuf;

namespace PirateX.Sync.ProtoSync
{
    /// <summary>
    /// proto协议描述同步服务
    /// 
    /// </summary>
    public class ProtobufService : IProtoService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// proto文件存放目录
        /// </summary>
        private static readonly string ProtoDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "protos");

        private static readonly string ModuleVersionIdFileName = Path.Combine(ProtoDir, "ModuleVersionId.txt");

        private static readonly string ProtoHashFile = Path.Combine(ProtoDir, "protoshash.json");

        private static string _currentModuleVersionId = string.Empty;

        private static Dictionary<string, string> _protoshash = new Dictionary<string, string>();

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
                var protoshash =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ProtoHashFile));
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
                File.WriteAllText(ProtoHashFile, JsonConvert.SerializeObject(_protoshash));
            }

            _isInitOk = true;
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
