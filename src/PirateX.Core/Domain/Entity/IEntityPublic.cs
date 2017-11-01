using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Domain.Entity
{
    /// <summary>
    /// 公共模型
    /// </summary>
    public interface IEntityPublic
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EntityPublicAttribute : Attribute
    {
        public string Name { get; }

        public EntityPublicAttribute(string name)
        {
            Name = name;
        }
    }
}
