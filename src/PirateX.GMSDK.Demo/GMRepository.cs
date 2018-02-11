using System;
using System.Collections.Generic;
using System.Linq;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo
{
    public class GMRepository : IGMRepository
    {
        public static IList<Activity> Activities = new List<Activity>();

        static GMRepository()
        {
            for (int i = 0; i < 50; i++)
            {
                Activities.Add(new Activity
                {
                    Id = i + 1
                         ,
                    Remark = $"{(i / 5) + 1}00.【数据展示】 测试活动[{i + 1}]"
                    ,
                    Days = new int[] { 1, 2, 3, 4, 5, 6, 7 }
                    ,
                    StartAt = new DateTime(2018, 1, 1)
                    ,
                    EndAt = new DateTime(2018, 10, 1)
                    ,
                    Args = "{}"
                });
            }
        }

        public IEnumerable<IActivity> GetActivities(int page = 1, int size = 10)
        {
            return Activities.OrderByDescending(item => item.Id).Skip((page - 1) * size).Take(size);
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


        private IList<Attachment> Attachments = new List<Attachment>()
        {
            new Attachment(){ Id =1 ,Name = "奖励1" , Rewards = new Reward{ Coin= 10,Gold = 100,PetId = 10 } },
            new Attachment(){ Id =2 ,Name = "奖励2" , Rewards = new Reward{ Coin= 10,Gold = 100,PetId = 10 } },
            new Attachment(){ Id =3 ,Name = "奖励3" , Rewards = new Reward{ Coin= 10,Gold = 100,PetId = 10 } },
            new Attachment(){ Id =4 ,Name = "奖励4" , Rewards = new Reward{ Coin= 10,Gold = 100,PetId = 10 } },
            new Attachment(){ Id =5 ,Name = "奖励5" , Rewards = new Reward{ Coin= 10,Gold = 100,PetId = 10 } },
        };
        public IEnumerable<Attachment> GetAttachments(int page = 1, int size = 10)
        {
            return Attachments.OrderByDescending(item => item.Id).Skip((page - 1) * size).Take(size);
        }

        public int AddAttachment(Attachment attachment)
        {
            attachment.Id = Attachments.Count() + 1;

            Attachments.Add(attachment);

            return attachment.Id;
        }

        public int RemoveAttachmentById(int id)
        {
            Attachments.Remove(new Attachment() { Id = id });

            return 1;
        }

        public Attachment GetAttachmentById(int id)
        {
            return Attachments.FirstOrDefault(item => item.Id == id);
        }
    }
}
