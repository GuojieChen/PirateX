using System;
using Autofac;

namespace PirateX.Core
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

        void SetUp(IContainer container, IServerSetting setting);
    }
}
