using System;
using Autofac;
using NetMQ;
using PirateX.Core.Online;
using PirateX.Protocol.Package;
using Topshelf.Logging;

namespace PirateX.Net.Actor.Actions
{
    public interface IAction:IDisposable
    {
        ILifetimeScope ServerReslover { get; set; }
        IMessageSender MessageSender { get; set; }
        LogWriter Logger { get; set; }
        ILifetimeScope Reslover { get; set; }
        /// <summary>
        /// 动作名称
        /// </summary>
        string Name { get; set; }

        IOnlineRole OnlieRole { get; set; }
        ActorContext Context { get; set; }

        void Execute();
    }
}
