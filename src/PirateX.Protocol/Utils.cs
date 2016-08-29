using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace PirateX.Protocol
{
    public class GameRandom : Random
    {
        private static readonly object _lockHelper = new object();
        private static readonly object _lockHelper2 = new object();
        private static readonly object _loadHelper3 = new object();

        private static readonly object _loadHelper4 = new object();

        public GameRandom(int seed) : base(seed)
        {

        }

        public override int Next(int minValue, int maxValue)
        {
            lock (_lockHelper)
            {
                return base.Next(minValue, maxValue);
            }
        }

        public override int Next(int maxValue)
        {
            lock (_lockHelper2)
            {
                return base.Next(maxValue);
            }
        }

        public override double NextDouble()
        {
            lock (_loadHelper3)
            {
                var next = base.Next(0, 1000);

                return next*1.0/1000;
                //return base.NextDouble();   
            }
        }

        public double NextDoubleGeneralThanZero()
        {
            lock (_loadHelper4)
            {
                var next = base.Next(1, 1000);

                return next * 1.0 / 1000;
                //return base.NextDouble();   
            }
        }
    }

    public static class Utils
    {
        public static GameRandom Random { get; private set; }

        static Utils()
        {
            var tick = DateTime.Now.Ticks;
            Random = new GameRandom((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        /// <summary>
        /// 获取数据中某一位的值
        /// </summary>
        /// <param name="input">传入的数据类型,可换成其它数据类型,比如Int</param>
        /// <param name="index">要获取的第几位的序号,从0开始</param>
        /// <returns>返回值为-1表示获取值失败</returns>
        public static int GetbitValue(byte input, int index)
        {
            return (input & ((uint)1 << index)) > 0 ? 1 : 0;
        }
        /// <summary>
        /// 位转成字节数组
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            if (bits.Count % 8 != 0)
                throw new ArgumentException("bits");

            var ret = new byte[bits.Length / 8];

            bits.CopyTo(ret, 0);

            Array.Reverse(ret);

            return ret;
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            return GetTimestamp(DateTime.UtcNow);
        }

        public static int GetTimestampAsSecond()
        {
            return (int)(GetTimestamp(DateTime.UtcNow) / 1000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeUtc"></param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dateTimeUtc)
        {
            return (dateTimeUtc.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000;
        }

        /// <summary>
        /// 是否是空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// 判断类型是否可空
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TypeAllowsNullValue(Type type)
        {
            return (!type.IsValueType || IsNullableValueType(type));
        }

        /// <summary> 计算MD5值 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5(string str)
        {
            StringBuilder sessionkeyBuilder = new StringBuilder();

            foreach (byte b in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)))
                sessionkeyBuilder.Append(b.ToString("x2"));

            return sessionkeyBuilder.ToString();

            /*
            var hashPasswordForStoringInConfigFile = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            if (hashPasswordForStoringInConfigFile != null)
                return hashPasswordForStoringInConfigFile.ToLower();
            return null;
            */
        }

        /// <summary>
        /// 空的返回
        /// </summary>
        public static readonly object EmptyResponse = new object();
        /// <summary>
        /// 将Utc时间转化成服务器设定时区的时间
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static DateTime ConvertUtcToLocal(DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);
        }
        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToLongString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss.fff"); 
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary> Post数据 并且读取返回
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string PostData(string postUrl, string paramData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(paramData);
            var webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";

            webReq.ContentLength = byteArray.Length;
            Stream newStream = webReq.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            var response = (HttpWebResponse)webReq.GetResponse();
            var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string ret = sr.ReadToEnd();
            sr.Close();
            response.Close();
            newStream.Close();
            return ret;
        }
        /// <summary>
        /// 构造一个Pvp授权码
        /// 
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="rid">玩家Rid</param>
        /// <param name="sourceId">
        /// 战斗类型
        /// 1  好友切磋
        /// 2  竞技场
        /// </param>
        /// <returns></returns>
        public static string PvpToken(long roomId, long rid, int sourceId)
        {
            var timestamp = Utils.GetTimestamp();
            var str = string.Format("{0}-{1}-{2}-{3}-{4}", rid, roomId, "1.0.0", sourceId, timestamp);
            var md5 = Utils.GetMd5(string.Format("{0}{1}{2}{3}", rid, roomId, timestamp, "pokemonx.glee"));

            return string.Format("{0}-{1}", str, md5);
            //return SerializeObject.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}-{1}", str, md5))); 
        }
        /// <summary>
        /// 获取本机绑定的IPv4地址，对于阿里云服务器 过滤了 10.  和 169. 开头的地址，因为需要的仅仅是外网地址。
        /// 对于开发的内网来说保留了 168. 的地址
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetIps()
        {
            var hostIpAddress = Dns.GetHostEntry(Dns.GetHostName());
            IList<string> ips = new List<string>();
            foreach (var ipAddress in hostIpAddress.AddressList)
            {
                var family = ipAddress.AddressFamily.ToString();
                var ip = ipAddress.ToString();
                if (Equals(family, "InterNetwork"))
                {
                    if (ip.StartsWith("10.") || ip.StartsWith("169.")) //TODO 这里的需要外部来指定过滤吗？ 后期再看吧 
                        continue;

                    ips.Add(ipAddress.ToString());
                }
            }
            return ips;
        }
        /// <summary> 比较版本号，新版本号大于就版本号就返回True
        /// </summary>
        /// <param name="strNewVersion">新版本号</param>
        /// <param name="strOldVersion">旧版本号</param>
        /// <returns></returns>
        public static bool IsNewVersion(string strNewVersion, string strOldVersion)
        {
            if ((string.IsNullOrEmpty(strNewVersion) || string.IsNullOrEmpty(strOldVersion))
                || strNewVersion.Split('.').Length != strOldVersion.Split('.').Length)

                return false; 

            var strNewV = strNewVersion.Split('.');
            var strOldV = strOldVersion.Split('.');
            for (int i = 0; i < strNewV.Length; i++)
            {
                if (Convert.ToInt32(strNewV[i]) > Convert.ToInt32(strOldV[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 将 int数组转化成字节数组
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static byte[] ListToBytes(IEnumerable<int> datas)
        {
            if (datas == null)
                return null;

            return Encoding.UTF8.GetBytes(string.Join(",", datas));
        }
        /// <summary>
        /// 将 long数组转化成字节数组
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static byte[] ListToBytes(IEnumerable<long> datas)
        {
            if (datas == null)
                return null;

            return Encoding.UTF8.GetBytes(string.Join(",", datas));
        }

        public static long[] BytesToList(byte[] datas)
        {
            if (datas == null)
                return null;

            var str = Encoding.UTF8.GetString(datas);

            return str.Split(new char[] { ',' }).Select(n => Convert.ToInt64(n)).ToArray();
        }

        public static IDictionary<string, object> ConvertToDic(this object obj)
        {
            if (obj == null)
                return null; 

            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
            {
                if (property.PropertyType == typeof(DateTime))
                    result.Add(property.Name, string.Format("{0:yyyy-MM-dd HH:mm:ss}", property.GetValue(obj)));
                else 
                    result.Add(property.Name, property.GetValue(obj));
            }

            return result;
        }

        public static IDictionary<string, bool> ConvertToDicUseForBool(this object obj)
        {
            if (obj == null)
                return null;

            IDictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
            {
                if (property.PropertyType != typeof(bool))
                    continue;

                var attrs = property.PropertyType.GetCustomAttributes(typeof (DisplayNameAttribute), false);
                if (attrs.Any())
                {
                    var dn = (DisplayNameAttribute) attrs[0];

                    result.Add(dn.DisplayName, (bool)property.GetValue(obj));

                }else 
                    result.Add(property.Name, (bool)property.GetValue(obj));
            }

            return result;
        }

        /// <summary> 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }
    }
}
