using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Core.Container
{

    [AttributeUsage(AttributeTargets.Interface)]
    public class DistrictConfigRegisterAttribute : Attribute
    {
        public Type RegisterType { get; private set; }

        public DistrictConfigRegisterAttribute(Type type)
        {
            if(!(typeof(IDistrictConfigRegister).IsAssignableFrom(type)))
                throw new InvalidCastException(type.FullName);

            RegisterType = type;
        }
    }

    public interface IDistrictConfigRegister
    {
        void Register(ContainerBuilder builder,IDistrictConfig config);


        void SetUp(IContainer container,IDistrictConfig config);
    }
}
