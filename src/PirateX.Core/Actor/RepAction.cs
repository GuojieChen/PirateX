using System;
using Autofac;
using StackExchange.Redis;

namespace PirateX.Core.Actor
{
    public abstract class RepAction : ActionBase
    {
    }

    public abstract class RepAction<TResponse>:ActionBase
    {
        public override void Execute()
        {
            var cachekey = GetResponseUrn();

            if (Context.Request.R)
            {
                //r == trye //拿之前的数据
                //获取和保存需要保持一致

                if (!string.IsNullOrEmpty(cachekey))
                    ResponseData = MessageSender.SendMessage(base.Context, GetFromCache(cachekey));
            }
            else
            {
                var response = Play();
                if (!Equals(response, default(TResponse)))
                {
                    //有值，返回

                    
                }
                else
                {
                    //返回默认值
                }


                ResponseData = MessageSender.SendMessage(base.Context, response);

                //缓存返回值
                if (!string.IsNullOrEmpty(cachekey))
                    SetToCache(cachekey,response);
            }

            AfterPlay();
        }

        //TODO 抽象到  IReqCache 中
        protected virtual TResponse GetFromCache(string key)
        {
            if (!base.Reslover.IsRegistered<IDatabase>())
                return default(TResponse) ;

            var data = Redis.StringGet(key);
            return RedisSerializer.Deserialize<TResponse>(data);
        }

        protected virtual void SetToCache(string key, TResponse response)
        {
            if (!base.Reslover.IsRegistered<IDatabase>())
                return ;

            var listurn = GetResponseListUrn();
            if (string.IsNullOrEmpty(listurn))
                return;

            Redis.StringSet(key, RedisSerializer.Serilazer(response),new TimeSpan(0,0,30));
            Redis.ListRightPush(GetResponseListUrn(), key);

            if (Redis.ListLength(listurn) >= 4)
            {//保存4条
                var removekey = Redis.ListLeftPop(listurn);
                Redis.KeyDelete(removekey.ToString());
            }
        }

        private string GetResponseUrn()
        {
            return $"rep:{base.Context.Token.Rid}:{base.Context.Request.C}";
        }

        private string GetResponseListUrn()
        {
            return $"replist:{base.Context.Token.Rid}";
        }

        public abstract TResponse Play();

        protected virtual void AfterPlay()
        {
            
        }

    }
}
