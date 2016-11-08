using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Net.Actor
{
    public interface IMessageSender
    {
        void SendMessage<T>(ActorContext context, T t);

        void SendMessage<T>(ActorContext context, string name, T t);
    }
}
