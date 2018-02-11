using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Middleware;

namespace PirateX.GMSDK
{
    public interface IGMRepository
    {
        #region 活动相关

        IEnumerable<IActivity> GetActivities(int page = 1, int size = 10);

        IActivity AddActivity(IActivity activity);

        int RemoveActivityById(int id);

        IActivity GetActivityById(int id);

        #endregion

        #region 奖励附件
        /// <summary>
        /// 获取活动附件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IEnumerable<Attachment> GetAttachments(int page = 1, int size = 10);
        /// <summary>
        /// 添加奖励附件
        /// </summary>
        /// <returns></returns>
        int AddAttachment(Attachment attachment);
        /// <summary>
        /// 移除附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int RemoveAttachmentById(int id);
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Attachment GetAttachmentById(int id);

        #endregion
    }
}
