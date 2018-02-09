﻿using System;
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
    }
}
