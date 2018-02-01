using System;

namespace PirateX.Core
{
    /// <summary> 泳衣配置模型的 索引键
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class ConfigIndex : Attribute
    {
        public string[] Names { get; private set; }

        public bool IsUnique { get; set; }

        public ConfigIndex(params string[] names)
        {
            Names = names;
        }

        public ConfigIndex(bool isUnique, params string[] names)
        {
            IsUnique = isUnique;
            Names = names;
        }
    }
}
