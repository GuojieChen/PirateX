using System;

namespace PirateX.Middleware.TaskSystem
{
    class TaskConditionDetectContext
    {
        private readonly long rid;
        private readonly ITaskConditionConfig condition;

        private ITaskConditionDetectStrategy strategy;
        public TaskConditionDetectContext(long rid, ITaskConditionConfig condition)
        {
            this.rid = rid;
            this.condition = condition;
            //TODO null 的处理
            strategy = GetStrategy(0);
        }

        public ITaskConditionDetectStrategy GetStrategy(int enumTaskCondition)
        {
            switch (enumTaskCondition)
            {
                //case EnumTaskCondition.AttackLand: return new AttackLandDetectStrategy();
                //case EnumTaskCondition.BuildingLevel: return new BuildingLevelDetectStrategy();
                //case EnumTaskCondition.BuildingOpened: return new BuildingOpendDetectStrategy();
                //case EnumTaskCondition.DetectLand: return new DetectLandDetectStrategy();
                //case EnumTaskCondition.HeroCount: return new HeroCountDetectStrategy();
                //case EnumTaskCondition.HeroMagic: return new HeroMagicDetectStrategy();
                //case EnumTaskCondition.LandCount: return new LandCountDetectStrategy();
                //case EnumTaskCondition.SoldierLevel: return new SoldierLevelDetectStrategy();
                //case EnumTaskCondition.SoldierNum: return new SoldierNumDetectStrategy();
                //case EnumTaskCondition.TechnologyLevel: return new TechnologyLevelDetectStrategy();
                //case EnumTaskCondition.LandMaxLevel: return new LandMaxLevelDetectStrategy();
                //case EnumTaskCondition.RegisterDays: return new RegisterDaysDetectStrategy();
                //case EnumTaskCondition.Bingding: return new BingdingDetectStrategy();
                //case EnumTaskCondition.HeroLevel: return new HeroLevelDetectStrategy();
                //case EnumTaskCondition.Action: return new ActionDetectStrategy();
                //case EnumTaskCondition.CityReName: return new CityReNameDetectStrategy();
                //case EnumTaskCondition.TaskMap: return new TaskMapDetectStrategy();
                //case EnumTaskCondition.TaskLetter: return new TaskLetterReadDetectStrategy();
                //case EnumTaskCondition.UserLevel: return new UserLevelDetectStrategy();
                //case EnumTaskCondition.UserName: return new UserNameDetectStrategy();
                //case EnumTaskCondition.RemainsCount: return new RemainsCountDetectStrategy();
                //case EnumTaskCondition.RemainsMaxLevel: return new RemainsMaxLevelDetectStrategy();
                //case EnumTaskCondition.MagicLevel: return new MagicLevelDetectStrategy();
                //case EnumTaskCondition.StoreCount: return new StoreCountDetectStrategy();
                //case EnumTaskCondition.NomalAction: return new NomalActionDetectStrategy();
                //case EnumTaskCondition.SubCityCount: return new CityCountDetectStrategy();
                //case EnumTaskCondition.PaymentCoin: return new PaymentDetectStrategy();
                //case EnumTaskCondition.TrainSoldier: return new TrainSoldierDetectStrategy();
                //case EnumTaskCondition.Upgrade:return new UpgradeDetectStrategy();
                //case EnumTaskCondition.MagicStudy:return new MagicStudyDetectStrategy();
                //case EnumTaskCondition.FirstPayment: return new FirstPaymentDetectStrategy();
                //case EnumTaskCondition.AttackCity: return new AttackCityDetectStrategy();
            }

            //NLogger.LogError("TaskConditionDetectContext", String.Format("任务检测条件[{0}]没有实现", condition.ID));
            //throw new NotImplementedException(String.Format("任务检测条件[{0}]没有实现", condition.ID));
            return null;
        }

        public bool Execute()
        {
            if (strategy == null)
                return false;

            //int v = strategy.GetDetectData(userID, condition);

            //return Operator(v, condition.Value, condition.Operate);

            return false; 
        }


        public int GetiV()
        {
            return 0;

            //return strategy.GetDetectData(userID, condition);
        }

        public static bool Operator(int real_value, int compare_value, int operator_type)
        {
            bool v = false;

            switch (operator_type)
            {
                case 1:
                    if (real_value > compare_value) v = true;
                    break;
                case 2:
                    if (real_value >= compare_value) v = true;
                    break;
                case 0:
                    if (real_value == compare_value) v = true;
                    break;
                case -1:
                    if (real_value < compare_value) v = true;
                    break;
                case -2:
                    if (real_value <= compare_value) v = true;
                    break;
            }

            return v;
        }
    }
}
