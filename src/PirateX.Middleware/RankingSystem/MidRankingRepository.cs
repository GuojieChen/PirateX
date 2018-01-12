using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Domain.Repository;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;

namespace PirateX.Middleware.RankingSystem
{
    public class MidRankingRepository: RepositoryBase
    {
        private string GetKey<TRanking>() where TRanking : IRanking
        {
            return typeof(TRanking).Name.ToLower();
        }
        private string GetPrivateKey<TRanking>(long rid) where TRanking : IRanking
        {
            return $"{typeof(TRanking).Name.ToLower()}:{rid}";
        }

        /// <summary>
        /// 添加元素到排行中
        /// </summary>
        /// <typeparam name="TRanking"></typeparam>
        /// <param name="ranking"></param>
        public void Store(IRanking ranking) 
        {
            var itemkey = GetPrivateKey<IRanking>(ranking.Rid);

            base.Redis.StringSet(itemkey, base.RedisSerializer.Serilazer(ranking));
            base.Redis.SortedSetAdd(GetKey<IRanking>(), itemkey, ranking.Score);
        }
        /// <summary>
        /// 从排行中移除
        /// </summary>
        /// <param name="ranking"></param>
        public void Remove(IRanking ranking) 
        {
            var itemkey = GetPrivateKey<IRanking>(ranking.Rid);
            base.Redis.SortedSetRemove(GetKey<IRanking>(), itemkey);
        }
        /// <summary>
        /// 获取排行列表
        /// </summary>
        /// <typeparam name="TRanking"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="order"></param>
        public List<TRanking> GetRankings<TRanking>(int page=1,int size = 20,Order order=Order.Descending) where TRanking:IRanking
        {
            var list = base.Redis.SortedSetRangeByRank(GetKey<TRanking>(), page * size, (page + 1) * size , order);

            var results = new List<TRanking>();

            var items = base.Redis.StringGet(list.Select(item => (RedisKey)Convert.ToString(item)).ToArray());

            foreach (var item in items)
                results.Add(base.RedisSerializer.Deserialize<TRanking>(item));

            return results;
        }
        /// <summary>
        /// 获取自己的排名
        /// </summary>
        /// <typeparam name="TRanking"></typeparam>
        /// <param name="rid"></param>
        /// <param name="order"></param>
        /// <returns>
        /// 为null的情况下标识不在排行中
        /// </returns>
        public long? MyRank<TRanking>(long rid, Order order = Order.Descending) where TRanking :IRanking
        {
            return base.Redis.SortedSetRank(GetKey<TRanking>(), GetPrivateKey<TRanking>(rid), order);
        }

    }
}
