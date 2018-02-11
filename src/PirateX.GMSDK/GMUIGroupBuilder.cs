using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    public class GMUIGroupBuilder
    {
        public List<string> Errors { get; private set; } = new List<string>();

        public Dictionary<string, object> Values { get; private set; } = new Dictionary<string, object>();

        private IEnumerable<IGMUIPropertyMap> _maps;
        private NameValueCollection _form;

        public GMUIGroupBuilder(IEnumerable<IGMUIPropertyMap> maps,NameValueCollection form)
        {
            _maps = maps;
            _form = form;
        }

        public void Build()
        {
            foreach (var propertyMap in _maps)
            {
                if (propertyMap is GMUIMapPropertyMap)
                {//对象
                    var item = propertyMap as GMUIMapPropertyMap;

                    //TODO 对象数组需要筛选出来。例如 A[0].Id=1&A[0].Name=xx&A[1].Id=2&A[2].Name=xxx
                    //测试数组长度
                    var len = _form.AllKeys.Count(key => key.StartsWith($"{item.Name}[") && key.EndsWith($"].{item.Map.PropertyMaps[0].Name}"));
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>(len);

                    for (int i = 0; i < len; i++)
                    {
                        var objValue = new Dictionary<string, object>();
                        foreach (var p in item.Map.PropertyMaps)
                        {
                            var key = $"{item.Name}[{i}].{p.Name}";
                            var value = _form[key];

                            var error = p.ValidateFunc?.Invoke(value);
                            if (!string.IsNullOrEmpty(error))
                                Errors.Add(error);

                            if (string.IsNullOrEmpty(value))
                                Errors.Add($"{item.DisplayName} 缺少值");
                            else
                                objValue.Add(p.Name, Convert.ChangeType(value, p.PropertyInfo.PropertyType));
                        }

                        list.Add(objValue);
                    }

                    Values.Add(item.Name, list);
                }
                else
                {
                    var value = _form[propertyMap.Name];
                    var error = propertyMap?.ValidateFunc?.Invoke(value);
                    if (!string.IsNullOrEmpty(error))
                        Errors.Add(error);

                    if (string.IsNullOrEmpty(value))
                        Errors.Add($"{propertyMap.DisplayName} 缺少值");
                    else
                        Values.Add(propertyMap.Name, value);
                }

            }
        }
    }
}
