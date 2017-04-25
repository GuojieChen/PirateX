using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Core.Container
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServerSettingRegisterAttribute : Attribute
    {
        public Type RegisterType { get; private set; }

        public ServerSettingRegisterAttribute(Type type)
        {
            if (!(typeof(IServerSettingRegister).IsAssignableFrom(type)))
                throw new InvalidCastException(type.FullName);

            RegisterType = type;
        }
    }


    public interface IServerSettingRegister
    {
        void Register(ContainerBuilder builder, IServerSetting setting);
    }
}
