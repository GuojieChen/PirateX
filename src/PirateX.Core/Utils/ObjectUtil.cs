using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;
using StackExchange.Redis;

namespace PirateX.Core
{
    public static class ObjectUtil
    {
        public static IDictionary<string, object> ToDictionary<T>(this T obj)
        {
            var dic = new Dictionary<string, object>();

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                dic.Add(propertyInfo.Name, propertyInfo.GetValue(obj));
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

        public static string GetMD5(this byte[] bytes)
        {
            var sessionkeyBuilder = new StringBuilder();
            foreach (byte b in MD5.Create().ComputeHash(bytes))
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
            if (string.IsNullOrEmpty(connectionString))
                return string.Empty;

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
                .Select(property =>
                    {
                        var value = property.GetValue(t);
                        return new HashEntry(property.Name, value?.ToString() ?? "");
                    })
                .ToArray();
        }

        public static T FromHashEntries<T>(this HashEntry[] entries)
        {
            var t = Activator.CreateInstance<T>();
            var type = typeof(T);
            foreach (var entry in entries)
            {
                var p = type.GetProperty(entry.Name);
                if (p == null || !entry.Value.HasValue)
                    continue;

                var value = entry.Value;
                if (p.PropertyType == typeof(bool))
                    value = Equals(value.ToString(), "TRUE");

                p.SetValue(t, Convert.ChangeType(value, p.PropertyType));
            }

            return t;
        }

        public static bool GetBit(this byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        public static int ToInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            return int.Parse(str);
        }

        [Obsolete("建议采用ToArray方法")]
        public static List<int> ToIntList(this string str)
        {
            return str.StringToList<int>();
        }
        [Obsolete("建议采用ToArray方法")]
        public static List<T> StringToList<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return new List<T>();

            if (str.StartsWith("{"))
                str = $"[{str.TrimStart('{').TrimEnd('}')}]";

            return JsonConvert.DeserializeObject<List<T>>(str);
        }
        
        public static T[] ToArray<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return new T[0];

            if (str.StartsWith("{"))
                str = str.TrimStart('{').TrimEnd('}');
            else if(str.StartsWith("["))
                str = str.TrimStart('[').TrimEnd(']');

            if (string.IsNullOrEmpty(str))
                return new T[0];

            var list = str.Split(new char[] { ',' }).Select(item => (T)Convert.ChangeType(item, typeof(T)));

            return list.ToArray();
        }

        public static string ArrayToString<T>(this IEnumerable<T> array)
        {
            return JsonConvert.SerializeObject(array);
        }

        public static string GetProto(this Type type)
        {
            var proto = typeof(Serializer).GetMethod("GetProto",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(type)
                .Invoke(null, null);

            return proto.ToString();
        }

        /// <summary>
        /// 短ID生成方法
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ShortIdGeneratoer(int len = 8)
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(len)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }

        public static int Max(this int[] ts)
        {
            var max = 0;

            foreach (var i in ts)
                max = Math.Max(max, i);

            return max;
        }


        public static void PrintBit(byte[] bytes)
        {
            for (var i = bytes.Length - 1; i >= 0; i--)
            {
                var b = bytes[i];
                for (var j = 7; j >= 0; j--)
                    Console.Write(b.GetBit(j) ? "1" : "0");

                Console.Write(',');
            }
            Console.WriteLine();
        }
    }
}
