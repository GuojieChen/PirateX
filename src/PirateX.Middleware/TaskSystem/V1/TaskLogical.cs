using System;
using System.Collections.Generic;
using NLog;
using PirateX.Core.Config;

namespace PirateX.Middleware
{
    public class TaskLogical : ILogical
    {
        private readonly IGameTask task;
        private IConfigReader _configReader;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _desNatural = true; 

        public TaskLogical(IConfigReader configReader,IGameTask task)
        {
            this._configReader = configReader;
            this.task = task;
            task.Conditions = new List<TaskCondition>();
        }

        //临时办法～～～～
        private int V;
        private int iV;
        private int rate; 

        public bool ProcessItem(string itemSymbol,bool inner)
        {
            int conditionId = Convert.ToInt32(itemSymbol);

            var condition = _configReader.SingleByIndexes<ITaskConditionConfig>(new { task.TaskId, conditionID = conditionId });
            if (condition == null)
            {
                Logger.Error($"_TaskCondition Data null! ID = {conditionId}");
                if(_desNatural)
                    task.Conditions.Add(new TaskCondition
                        {
                            _ID = conditionId,
                            OK = false, 
                            // TODO _Condition = _TaskConditionDao.Select(task._ID, conditionID)
                        });
                return false;
            }
            bool flag = false;
            TaskCondition userCondition = null;

            //记录有掉落道具的条件 对照玩家所拥有的数量。
            int iv = 0;

            if (task.StaticConditions != null && task.StaticConditions.Contains(conditionId))
            {//静态数据 并且已经完成了
                //task.Conditions.Add(new TaskCondition { _ID = conditionID, OK = true, _Condition = _TaskConditionDao.Select(task._ID, conditionID) });
                //return true; 
                userCondition = new TaskCondition
                    {
                        _ID = conditionId,
                        OK = true,
                        //TODO _Condition = _TaskConditionDao.Select(task._ID, conditionId)
                    };
                flag = true;

                iv = condition.V; 
            }
            else
            {//非静态条件
                TaskConditionDetectContext context = new TaskConditionDetectContext(task.Rid, condition);
                //flag = context.Execute();
                iv = context.GetiV();
                flag = TaskConditionDetectContext.Operator(iv, condition.V, condition.Operate); 

                if (flag && condition.IsStatic)
                {//静态数据 保存已经检测完毕
                    
                        try
                        {
                        //TODO 

                            //if (TaskDao.InsertTaskStaticCondition(task.ID, condition.ID))
                            //{
                            //    if (task.StaticConditions == null)
                            //        task.StaticConditions = new List<int>();

                            //    task.StaticConditions.Add(condition.ID);
                            //}
                        }
                        catch (Exception)
                        {

                            //TODO 
                            //Logger.Error("TaskStaticCondition error {0},{1}", task.ID, condition.ID);
                        }
                }

                userCondition = new TaskCondition
                    {
                        _ID = conditionId,
                        OK = flag,
                        //TODO  _Condition = _TaskConditionDao.Select(task._ID, conditionId)
                    };

            }
            /*
            //地图点信息
            if (condition.BattleData != null)
            {
                TaskMap taskmap = null;

                if (!inner)
                {//之计算
                    taskmap = TaskMapDao.SelectTaskMap(task.UserID, condition.ID);
                    if (taskmap != null)
                        userCondition.Point = new TaskMapPoint { X = taskmap.X, Y = taskmap.Y };

                    userCondition._Condition.BattleData = condition.BattleData;
                }
                else
                {//外部
                    taskmap = TaskMapDao.SelectTaskMap(task.UserID, condition.ID);
                    if (taskmap!=null)
                        MutiPointFirstOne = new TaskMapPoint{X = taskmap.X,Y = taskmap.Y };
                }
            }

             // 掉落数量
            _TaskConditionReward reward = _TaskConditionDao.SelectConditionReward(condition.ID);
            if(reward!=null)
            {
                this.Value = condition.Value;
                this.iV = iv;
                if(inner)
                {
                    this.rate = reward.Rate; 
                }else
                {
                    userCondition._Condition.Des =
                        condition.Des.IndexOf("({X},{Y})", System.StringComparison.Ordinal) < 0 ? condition.Des :
                        condition.Des.Insert(condition.Des.IndexOf("({X},{Y})", System.StringComparison.Ordinal), String.Format(" {0}/{1}  ", iV, Value));
                    userCondition._Condition.Rate = reward.Rate; 

                }
            } 

            //钻石
            if (!string.IsNullOrEmpty(condition.Des) && condition.Des.IndexOf("{Coin}", StringComparison.Ordinal) >= 0)
            {
                object o = ServerCache.GetUserPaymentCount(task.UserID);
                if (o == null)
                    o = TaskDetectDao.GetPaymentAmount(task.UserID);
                userCondition._Condition.Des = condition.Des.Replace("{Coin}", Convert.ToString(o)); 
            }
            //用户昵称
            if (!string.IsNullOrEmpty(condition.Des) && condition.Des.IndexOf("{UserName}", StringComparison.Ordinal) >= 0)
            {
                User user = UserDao.Select(task.UserID); 

                if(user!=null)
                {
                    userCondition._Condition.Des = condition.Des.Replace("{UserName}", user.Name);
                }
            }

            if ( !inner)
            {
                task.Conditions.Add(userCondition);
            }
            */
            return flag;
        }

        public void ConditionBlock(string key, bool value,bool comp)
        {
            if (!comp)
                return;

            if(task.Conditions == null)
                task.Conditions = new List<TaskCondition>();
            /* TODO 
            var des = _TaskConditionDesDao.Select(key); 
            if (des != null)
            {
                task.Conditions.Add(new TaskCondition
                {
                    _ID = des.Id,
                    OK = value,
                    _Condition = new _TaskCondition
                        {
                            ID = des.Id,
                            Des = 
                            des.Des.IndexOf("({X},{Y})", System.StringComparison.Ordinal)<0?des.Des:
                            des.Des.Insert(des.Des.IndexOf("({X},{Y})", System.StringComparison.Ordinal), String.Format(" {0}/{1}  ", iV, Value)),
                            Rate = rate
                        },


                    Point = MutiPointFirstOne,
                        
                });    
            }
            else
            {
                Logger.Error("任务[{0}]的条件表达式[{1}]缺少",task._ID,key);
            }


            */
        }
    }
}
