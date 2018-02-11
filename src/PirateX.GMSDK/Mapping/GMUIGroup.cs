using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Mapping
{
    public class GMUIGroup
    {
        public string ObjectName { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<IGMUIPropertyMap> Maps { get; set; }

        public bool CanMulti { get; set; }

        public string Id { get; set; }

        public static List<GMUIGroup> ConvertToGMUIGroupList(IEnumerable<IGMUIPropertyMap> maps)
        {
            int groupid = 1;
            var groups = maps.Where(item => !item.GetType().IsAssignableFrom(typeof(GMUIMapPropertyMap)))
                .GroupBy(item => item.GroupName).OrderBy(item => item.Key)
                .Select(item => new GMUIGroup()
                {
                    Id = $"uigroup_{groupid++}",
                    DisplayName = item.Key,
                    Maps = item.AsEnumerable<IGMUIPropertyMap>()
                }).ToList();
            //自定义类型
            groups.AddRange(maps.Where(item => item.GetType().IsAssignableFrom(typeof(GMUIMapPropertyMap)))
                .Select(item => new GMUIGroup()
                {
                    Id = $"uigroup_{groupid++}",
                    ObjectName = item.Name,
                    DisplayName = item.GroupName,
                    Maps = (item as GMUIMapPropertyMap).Map.PropertyMaps,
                    CanMulti = item.PropertyInfo.PropertyType.IsArray
                }));

            return groups;
        }
    }
}
