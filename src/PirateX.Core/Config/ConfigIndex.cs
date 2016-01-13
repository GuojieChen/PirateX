using System;

namespace PirateX.Core.Config
{
    /// <summary> 泳衣配置模型的 索引键
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigIndex : Attribute
    {
        public string[] Names { get; private set; }

        public ConfigIndex(params string[] names)
        {
            Names = names; 
        }
    }
}
