using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core;

namespace PirateX.Schedule.Worker
{
    [CronSchedule("0 0/1 * 1/1 * ? *")]
    public class TestJob: GameJobTaskBase<TestJob>
    {
        protected override void SubJob(IDistrictContainer container, IDistrictConfig config)
        {
            Console.WriteLine(DateTime.UtcNow);
        }
    }
}
