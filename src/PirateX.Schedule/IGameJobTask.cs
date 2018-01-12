using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using PirateX.Core.Container;

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
                Logger.Debug(string.Format("{0} START!", typeof(T).Name));

            if (!Continue())
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug(string.Format("{0} NOT Continue!", typeof(T).Name));

                return;
            }

            SubJob(container, config);

            if (Logger.IsDebugEnabled)
                Logger.Debug(string.Format("{0} DONE!", typeof(T).Name));
        }

        protected virtual bool Continue()
        {
            return true;
        }

        protected abstract void SubJob(IDistrictContainer container, IDistrictConfig config);


    }
}
