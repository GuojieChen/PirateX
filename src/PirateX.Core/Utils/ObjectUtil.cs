using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using StackExchange.Redis;

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

        public static string GetMD5(this string str)
        {
            var sessionkeyBuilder = new StringBuilder();

            foreach (byte b in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)))
                sessionkeyBuilder.Append(b.ToString("x2"));

            return sessionkeyBuilder.ToString();
        }

        public static byte[] ToProtobuf<T>(this T t) 
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, t);
                return ms.ToArray();
            }
        }

        public static T FromProtobuf<T>(this byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        /// <summary>
        /// 获取配置连接的信息摘要
        /// 这个后期是否需要非内置？
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static string GetConfigDbKey(this string connectionString)
        {
            var items = connectionString.Split(new char[] { ';' });
            var builder = new StringBuilder();
            foreach (var item in items)
            {
                var ss = item.Split(new char[] { '=' });
                switch (ss[0].Trim().ToLower())
                {
                    case "database":
                    case "server":
                        builder.Append(ss[1].Trim().ToLower());
                        break;
                }
            }

            return builder.ToString();
        }


        public static HashEntry[] ToHashEntries<T>(this T t)
        {
            return t.GetType()
                .GetProperties()
                .Select(property => new HashEntry(property.Name, property.GetValue(t).ToString()))
                .ToArray();
        }

        public static T FromHashEntries<T>(this HashEntry[] entries)
        {
            var t = Activator.CreateInstance<T>();
            var type = typeof(T);
            foreach (var entry in entries)
            {
                var p = type.GetProperty(entry.Name);
                if(p == null)
                    continue;

                p.SetValue(t,Convert.ChangeType(entry.Value,p.PropertyType));
            }

            return t;
        }
    }
}
