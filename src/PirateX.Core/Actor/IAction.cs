using System;
using Autofac;
using NLog;
using PirateX.Core.Session;

namespace PirateX.Core.Actor
{
    public interface IAction:IDisposable
    {
        ILifetimeScope ServerReslover { get; set; }
        IMessageSender MessageSender { get; set; }
        Logger Logger { get; set; }
        ILifetimeScope Reslover { get; set; }
        /// <summary>
        /// 动作名称
        /// </summary>
        string Name { get; set; }

        //PirateSession Session { get; set; }
        ActorContext Context { get; set; }

        void Execute();

        byte[] ResponseData { get; set; }

    }
}
