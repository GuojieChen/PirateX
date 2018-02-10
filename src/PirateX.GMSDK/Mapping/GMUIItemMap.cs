using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PirateX.Middleware;

namespace PirateX.GMSDK.Mapping
{

    
    public interface IGMUIItemMap
    {
        string Name { get; }

        string Des { get; }
        IList<IGMUIPropertyMap> PropertyMaps { get; }
    }

    
    public interface IGMUIItemMap<TGMUIItem> : IGMUIItemMap
    {
    }
    /// <summary>
    /// 对应了一个类，比如活动奖励
    /// </summary>
    /// <typeparam name="TGMUIItem"></typeparam>
    public  abstract class GMUIItemMap<TGMUIItem> : IGMUIItemMap<TGMUIItem>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Des { get; protected set; }
        
        protected GMUIItemMap()
        {
            PropertyMaps = new List<IGMUIPropertyMap>();
        }

        public IList<IGMUIPropertyMap> PropertyMaps { get; }

        protected TPropertyMap Map<TPropertyMap>(string groupname,Expression<Func<TGMUIItem, object>> expression)
            where TPropertyMap : IGMUIPropertyMap
        {
            var info = (PropertyInfo)ReflectionHelper.GetMemberInfo(expression);
            var propertyMap = Activator.CreateInstance<TPropertyMap>();
            propertyMap.PropertyInfo = info;
            //if (info.PropertyType.IsArray)
            //    propertyMap.CanMulti(true);

            ThrowIfDuplicateMapping(propertyMap);
            PropertyMaps.Add(propertyMap);
            return propertyMap;
        }

        protected TPropertyMap Map<TPropertyMap>(Expression<Func<TGMUIItem, object>> expression)
        where TPropertyMap :IGMUIPropertyMap
        {
            return Map<TPropertyMap>(string.Empty, expression);
        }

        private void ThrowIfDuplicateMapping(IGMUIPropertyMap map)
        {
            if (PropertyMaps.Any(p => p.Name == map.Name))
            {
                throw new Exception($"Duplicate mapping detected. Property '{map.Name}' is already mapped to name '{map.Name}'.");
            }
        }
    }
}
