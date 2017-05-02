using System;
using Autofac;
using PirateX.Core.Domain.Uow;
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

                if(!string.IsNullOrEmpty(cachekey))
                    MessageSender.SendMessage(base.Context, GetFromCache(cachekey) );
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


                MessageSender.SendMessage(base.Context, response);

                //缓存返回值
                if (!string.IsNullOrEmpty(cachekey))
                    SetToCache(cachekey,response);
            }

            AfterPlay();
        }

        protected UnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(this.Reslover);
        }

        //TODO 抽象到  IReqCache 中
        protected virtual TResponse GetFromCache(string key)
        {
            if (!base.ServerReslover.IsRegistered<IDatabase>())
                return default(TResponse) ;

            var data = Redis.StringGet(key);
            return RedisSerializer.Deserialize<TResponse>(data);
        }

        protected virtual void SetToCache(string key, TResponse response)
        {
            if (!base.ServerReslover.IsRegistered<IDatabase>())
                return ;

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
            if (base.Session == null)
                return string.Empty;

            return $"rep:{base.Session.Id}:{base.Context.Request.C}_{base.Context.Request.O}";
        }

        private string GetResponseListUrn()
        {
            if (base.Session == null)
                return string.Empty;

            return $"replist:{base.Session.Id}";
        }

        public abstract TResponse Play();

        protected virtual void AfterPlay()
        {
            
        }

    }
}
