using System;

namespace PirateX.Core.Actor
{
    public abstract class RepAction : ActionBase
    {
        
    }

    public abstract class RepAction<TResponse>:ActionBase
    {
        public override void Execute()
        {

            var response = Play();

            var cachekey = GetResponseUrn();

            if (Context.Request.R)
            {
                //r == trye //拿之前的数据
                //获取和保存需要保持一致

                if(!string.IsNullOrEmpty(cachekey))
                    MessageSender.SendMessage(base.Context, GetFromCache(cachekey) );
            }
            else
            {
                if (!Equals(response, default(TResponse)))
                {
                    //有值，返回

                    
                }
                else
                {
                    //返回默认值
                }


                MessageSender.SendMessage(base.Context, response);

                //缓存返回值
                if (!string.IsNullOrEmpty(cachekey))
                    SetToCache(cachekey,response);
            }
        }


        protected virtual TResponse GetFromCache(string key)
        {
            var data = Redis.StringGet(key);
            return RedisSerializer.Deserialize<TResponse>(data);
        }

        protected virtual void SetToCache(string key, TResponse response)
        {
            var listurn = GetResponseListUrn();
            if (string.IsNullOrEmpty(listurn))
                return;

            var trans = Redis.CreateTransaction();
            trans.StringSetAsync(key, RedisSerializer.Serilazer(response),new TimeSpan(0,0,30));
            trans.ListRightPushAsync(GetResponseListUrn(), key);
            trans.Execute();

            if (Redis.ListLength(listurn) >= 4)
            {//保存4条
                var removekey = Redis.ListLeftPop(listurn);
                Redis.KeyDelete(removekey.ToString());
            }
        }


        private string GetResponseUrn()
        {
            if (base.OnlieRole == null)
                return string.Empty;

            return $"rep:{base.OnlieRole.Id}:{base.Context.Request.C}_{base.Context.Request.O}";
        }

        private string GetResponseListUrn()
        {
            if (base.OnlieRole == null)
                return string.Empty;

            return $"replist:{base.OnlieRole.Id}";
        }

        public abstract TResponse Play();
    }
}
