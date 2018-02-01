using System;
using System.Collections.Generic;
using System.Linq;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class ChatRepository:PublicRepository
    {
        /// <summary>
        /// 默认保存的聊天数
        /// </summary>
        public static int Size = 30;

        private string GetKey(int channelid)
        {
            return $"{typeof(IChat).Name.ToLower()}:{channelid}";
        }

        public IChat Insert(IChat chat)
        {
            var key = GetKey(chat.ChannelId);

            chat.Id = DateTime.UtcNow.GetTimestamp();
            base.Redis.ListRightPush(key, base.RedisSerializer.Serilazer(chat));

            if (base.Redis.ListLength(key) > Size)
                base.Redis.ListLeftPop(key);

            return chat;
        }

        public IEnumerable<TChat> GetChats<TChat>(int channelid) where TChat :IChat
        {
            var list = base.Redis.ListRange(GetKey(channelid));

            return list.Select(item => base.RedisSerializer.Deserialize<TChat>(item));
        }
    }
}
