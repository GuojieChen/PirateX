using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Utils
{
    public static class ObjectUtil
    {
        public static IDictionary<string, object> ToDictionary<T>(this T obj)
        {
            var dic = new Dictionary<string,object>();

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                dic.Add(propertyInfo.Name,propertyInfo.GetValue(obj));
            }

            return dic;
        }

        public static string ToLogString<T>(this T t)
        {
            var logBuilder = new StringBuilder();
            logBuilder.AppendLine();
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                logBuilder.AppendLine($"{propertyInfo.Name,-20}\t{propertyInfo.GetValue(t)}");
            }

            return logBuilder.ToString();
        }
    }
}
