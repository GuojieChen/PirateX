using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PirateX.Core;

namespace PirateX.Schedule.Worker
{
    public class WorkerService:ScheduleWorkerService<WorkerService>
    {
        public WorkerService(string frontendConnect, string requestConnectionString) 
            : base(frontendConnect, requestConnectionString)
        {
        }

        protected override IDistrictContainer GetDistrictContainer(JObject jObject)
        {
            return new GameContainer(jObject["ConfigString"].Value<string>());
        }
    }
}
