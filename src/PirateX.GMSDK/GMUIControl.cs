using PirateX.GMSDK.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PirateX.GMSDK
{
    /// <summary>
    /// 控件
    /// </summary>
    public class GMUIControl
    {
        public string Control { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Tips { get; set; }

        public bool IsRequired { get; set; }

        public string DevaultValue { get; set; }

        public int OrderId { get; set; }

        public IEnumerable<GMUIDataDropdown> Data_DropdownList { get; set; }

        public IEnumerable<GMUIDataCheckbox> Data_CheckboxList { get; set; }
    }

    public class GMUIControlGroup
    {
        /// <summary>
        /// 有值的情况下标识是一个对象，Controls中的所有属性归该对象
        /// </summary>
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool CanMulti { get; set; }

        public IEnumerable<GMUIControl> Controls { get; set; }


        public static IEnumerable<GMUIControlGroup>  ConvertToGroups(IGMUIItemMap map)
        {
            List<GMUIControlGroup> list = new List<GMUIControlGroup>();

            list.AddRange(map.PropertyMaps.Where(item => !item.GetType().IsAssignableFrom(typeof(GMUIMapPropertyMap)))
                .GroupBy(item => item.GroupName).OrderBy(item => item.Key).Select(item => new GMUIControlGroup()
            {
                DisplayName = item.Key,
                Controls = item.Select(x=>
                {
                    var r = new GMUIControl()
                    {
                        Control = x.Control,
                        Name = x.Name,
                        DisplayName = x.DisplayName,
                        Tips = x.Tips,
                        IsRequired = x.IsRequired,
                        DevaultValue = x.DevaultValue,
                        OrderId = x.OrderId,
                    };


                    if (x is GMUIDropdownPropertyMap)
                    {
                        r.Data_DropdownList = (x as GMUIDropdownPropertyMap).ListDataProvider.GetListItems();
                    }
                    else if (x is GMUICheckBoxPropertyMap)
                    {
                        r.Data_CheckboxList = (x as GMUICheckBoxPropertyMap).CheckedDataProvider.GetCheckedItems();
                    }
                    else if (x is GMUIListCheckBoxPropertyMap)
                    {
                        r.Data_CheckboxList = (x as GMUIListCheckBoxPropertyMap).CheckedDataProvider.GetCheckedItems();
                    }
                    return r;
                })
            }));

            list.AddRange(map.PropertyMaps.Where(item => item.GetType().IsAssignableFrom(typeof(GMUIMapPropertyMap)))
                .Select(item=>new GMUIControlGroup()
                {
                    Name = item.Name,
                    DisplayName = item.DisplayName,
                    Controls = (item as GMUIMapPropertyMap).Map.PropertyMaps.Select(x=> 
                    {
                        var r = new GMUIControl()
                        {
                            Control = x.Control,
                            Name = x.Name,
                            DisplayName = x.DisplayName,
                            Tips = x.Tips,
                            IsRequired = x.IsRequired,
                            DevaultValue = x.DevaultValue,
                            OrderId = x.OrderId,
                        };

                        if (x is GMUIDropdownPropertyMap)
                        {
                            r.Data_DropdownList = (x as GMUIDropdownPropertyMap).ListDataProvider.GetListItems();
                        }
                        else if (x is GMUICheckBoxPropertyMap)
                        {
                            r.Data_CheckboxList = (x as GMUICheckBoxPropertyMap).CheckedDataProvider.GetCheckedItems();
                        }
                        else if (x is GMUIListCheckBoxPropertyMap)
                        {
                            r.Data_CheckboxList = (x as GMUICheckBoxPropertyMap).CheckedDataProvider.GetCheckedItems();
                        }

                        return r; 
                    }),
                    CanMulti = item.PropertyInfo.PropertyType.IsArray || typeof(IEnumerable).IsAssignableFrom(item.PropertyInfo.PropertyType)
                }));

            return list; 
        }
    }
}