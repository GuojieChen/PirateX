using System;
using System.Collections.Generic;
using System.Linq;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo
{
    public class GMRepository:IGMRepository
    {
        public static IList<Activity> Activities = new List<Activity>();

        static GMRepository()
        {
            for (int i = 0; i < 50; i++)
            {
                Activities.Add(new Activity{
                    Id = i +1 
                         ,Remark = $"{(i / 5)+1}00.【数据展示】 测试活动[{i+1}]"
                    ,Days = new int[]{1,2,3,4,5,6,7}
                    ,StartAt = new DateTime(2018,1,1)
                    ,EndAt = new DateTime(2018,10,1)
                    ,Args = "{}"});
            }
        }

        public IEnumerable<IActivity> GetActivities(int page = 1,int size = 10)
        {
            return Activities.OrderByDescending(item => item.Id).Skip((page-1) *size).Take(size);
        }

        public IActivity AddActivity(IActivity activity)
        {
            activity.Id = Activities.Count + 1;
            Activities.Add((Activity)activity);

            return activity;
        }

        public int RemoveActivityById(int id)
        {
            return 0;
        }

        public IActivity GetActivityById(int id)
        {
            return Activities.FirstOrDefault(item => item.Id == id);
        }
    }
}
