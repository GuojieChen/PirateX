using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using Newtonsoft.Json;
using NLog;
using PirateX.Core;
using PirateX.Core.Domain.Entity;
using PirateX.Protocol.ProtoSync;
using ProtoBuf;

namespace PirateX.Service
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


        private static string CurrentModuleVersionId = string.Empty;

        private static Dictionary<string, string> Protoshash = new Dictionary<string, string>();

        private bool _isInitOk = false;
        public void Init(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(item => typeof(IEntity).IsAssignableFrom(item));

            if (!Directory.Exists(ProtoDir))
                Directory.CreateDirectory(ProtoDir);

            if (File.Exists(ModuleVersionIdFileName))
                CurrentModuleVersionId = File.ReadAllText(ModuleVersionIdFileName);
            else
                using (var f = File.Create(ModuleVersionIdFileName))
                {

                }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"previous ModuleVersionId is {CurrentModuleVersionId}");

            if (File.Exists(ProtoHashFile))
            {
                var protoshash =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ProtoHashFile));
                if (protoshash != null)
                    Protoshash = protoshash;
            }
            else
            {
                using (var f = File.Create(ProtoHashFile))
                {
                }
            }

            if (!Equals(assembly.ManifestModule.ModuleVersionId.ToString(), CurrentModuleVersionId))
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug("genterating proto files...");

                foreach (var type in types)
                {
                    var guid = type.GUID.ToString();

                    if (Protoshash.ContainsKey(type.Name) && Equals(Protoshash[type.Name], guid))
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

                    if (Protoshash.ContainsKey(type.Name))
                        Protoshash[type.Name] = guid;
                    else
                        Protoshash.Add(type.Name, guid);
                }

                CurrentModuleVersionId = assembly.ManifestModule.ModuleVersionId.ToString();
                File.WriteAllText(ModuleVersionIdFileName, assembly.ManifestModule.ModuleVersionId.ToString());
                File.WriteAllText(ProtoHashFile, JsonConvert.SerializeObject(Protoshash));
            }

            _isInitOk = true;
        }

        public string GetProtosHash()
        {
            return CurrentModuleVersionId;
        }

        public IDictionary<string, string> GetProtosHashDic(Assembly assembly)
        {
            return Protoshash;
        }

        /// <summary> 生成签名
        /// </summary>
        /// <param name="proto"></param>
        /// <returns></returns>
        private string GetMD5String(string proto)
        {
            var bytes = Encoding.Default.GetBytes(proto);

            var md5 = new MD5CryptoServiceProvider();
            var output = md5.ComputeHash(bytes);

            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }
    }
}
