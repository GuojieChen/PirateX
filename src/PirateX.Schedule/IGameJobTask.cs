using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using PirateX.Core;

namespace PirateX.Schedule
{
    public interface IGameJobTask
    {
        JObject ConfigJson { get; set; }
        void Execute(IDistrictContainer container, IDistrictConfig config);
    }

    public abstract class GameJobTaskBase<T> : IGameJobTask
    {
        public JObject ConfigJson { get; set; }
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public void Execute(IDistrictContainer container, IDistrictConfig config)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"TASK [{typeof(T).Name}] START!");

            if (!Continue())
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"{typeof(T).Name} NOT Continue!");

                return;
            }

            SubJob(container, config);

            if (Logger.IsDebugEnabled)
                Logger.Debug($"TASK [{typeof(T).Name}] DONE!");
        }

        protected virtual bool Continue()
        {
            return true;
        }

        protected abstract void SubJob(IDistrictContainer container, IDistrictConfig config);


    }
}
