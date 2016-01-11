//using System;

//namespace PirateX
//{
//    internal static class ShareConfig
//    {
//        private static string _shareDb;
//        private static string _redisHost;
//        private static string _onlineRedisHost;
//        private static bool? _alterTable;
//        private static bool? _single;

//        /// <summary> 共享数据库连接字符串
//        /// </summary>
//        public static string ShareDb
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_shareDb))
//                    _shareDb = System.Configuration.ConfigurationManager.AppSettings.Get("ServerDb");

//                return _shareDb; 
//            }
//        }
        
//        /// <summary>
//        /// 共享Redis地址，这个是局域网内的,共享的Db默认是0
//        /// </summary>
//        public static string RedisHost
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_redisHost))
//                    _redisHost = System.Configuration.ConfigurationManager.AppSettings.Get("RedisHost");

//                return _redisHost; 
//            }
//        }

//        /// <summary>
//        /// 存放服务器和玩家在线信息的缓存地址 默认用Db 0 
//        /// </summary>
//        public static string OnlineRedisHost
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_redisHost))
//                    _onlineRedisHost = System.Configuration.ConfigurationManager.AppSettings.Get("OnlineRedisHost") ?? "localhost";

//                return _onlineRedisHost;
//            }
//        }

//        /// <summary> 是否需要自动更新表
//        /// </summary>
//        public static bool AlterTable
//        {
//            get
//            {
//                if (_alterTable == null)
//                    _alterTable =
//                        Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("AlterTable"));

//                return _alterTable != null && _alterTable.Value;
//            }
//        }
//        /// <summary>
//        /// 是否作为单服处理
//        /// </summary>
//        public static bool Single
//        {
//            get
//            {
//                if(_single == null)
//                    _single =
//                        Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("Single"));

//                return _single == null || _single.Value;
//            }
//        }
//    }
//}
