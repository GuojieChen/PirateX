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

        private IEnumerable<GMUIControlGroup> _groups;
        private NameValueCollection _form;

        public GMUIGroupBuilder(IEnumerable<GMUIControlGroup> groups, NameValueCollection form)
        {
            _groups = groups;
            _form = form;
        }

        public void Build()
        {
            foreach (var group in _groups)
            {
                if (!string.IsNullOrEmpty(group.Name))
                {//对象
                    //var obj = Activator.CreateInstance(item.PropertyInfo.PropertyType.GetElementType());
                    var list = new List<Dictionary<string, string>>();

                    //TODO 对象数组需要筛选出来。例如 A[0].Id=1&A[0].Name=xx&A[1].Id=2&A[2].Name=xxx
                    //测试数组长度

                    var len = _form.AllKeys.Count(key => key.StartsWith($"{group.Name}[") && key.EndsWith($"].{group.Controls[0].Name}"));
                    for (int i = 0; i < len; i++)
                    {
                        var objValue = new Dictionary<string, string>();
                        foreach (var p in group.Controls)
                        {
                            var key = $"{group.Name}[{i}].{p.Name}";
                            var value = _form[key];
                            p.DevaultValue = value;

                            //var error = p.ValidateFunc?.Invoke(value);
                            //if (!string.IsNullOrEmpty(error))
                            //    Errors.Add(error);

                            objValue.Add(p.Name, value);
                        }
                        list.Add(objValue);
                    }

                    Values.Add(group.Name, list);
                }
                else
                {
                    foreach (var c in group.Controls)
                    {
                        var value = _form[c.Name];

                        if (string.IsNullOrEmpty(value))
                            Errors.Add($"{c.DisplayName} 缺少值");
                        else
                        {
                            if (Values.ContainsKey(c.Name))
                                Values[c.Name] = value;
                            else 
                                Values.Add(c.Name, value);
                        }

                        c.DevaultValue = value;
                    }
                }
            }
        }

    }
}
