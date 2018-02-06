using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json.Linq;
using PirateX.Core;
using ServiceStack;

namespace PirateX.Schedule.Worker
{
    public class GameContainer: DistrictContainer<GameContainer>
    {


        private ServerSetting _serverSetting;
        private IDistrictConfig[] _districtConfigs;
        private IDictionary<string, string> _connections;

        public GameContainer(string configUrl)
        {
            var config = JObject.Parse(configUrl.GetJsonFromUrl());

            if (Log.IsTraceEnabled)
                Log.Trace($"\r\n{config}");


            _serverSetting = config["ServerSetting"].ToObject<ServerSetting>();
            _districtConfigs = config["DistrictConfigs"].ToObject<DistrictConfig[]>();
            _connections = config["Connections"].ToObject<Dictionary<string, string>>();
        }

        public override IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            return _districtConfigs;
        }

        public override IServerSetting GetServerSetting()
        {
            return _serverSetting;
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            if (id != 1)
                return null;

            return _districtConfigs.FirstOrDefault(item => item.Id == id);
        }


        protected override void BuildServerContainer(ContainerBuilder builder)
        {

        }

        protected override void BuildDistrictContainer(ContainerBuilder builder)
        {

        }
    }


    public class DistrictConfig
        : IDistrictConfig
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string SecretKey { get; set; }
        public int TargetId { get; set; }
    }

    public class ServerSetting
        : IServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
