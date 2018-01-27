using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

    public class GMUIItemMap<TGMUIItem> : IGMUIItemMap<TGMUIItem>
    {

        public string Name { get; protected set; }

        public string Des { get; protected set; }
        
        protected GMUIItemMap()
        {
            PropertyMaps = new List<IGMUIPropertyMap>();
        }

        public IList<IGMUIPropertyMap> PropertyMaps { get; }

        protected TPropertyMap Map<TPropertyMap>(Expression<Func<TGMUIItem, object>> expression)
        where TPropertyMap :IGMUIPropertyMap
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

        private void ThrowIfDuplicateMapping(IGMUIPropertyMap map)
        {
            if (PropertyMaps.Any(p => p.Name == map.Name))
            {
                throw new Exception($"Duplicate mapping detected. Property '{map.Name}' is already mapped to name '{map.Name}'.");
            }
        }
    }
}
