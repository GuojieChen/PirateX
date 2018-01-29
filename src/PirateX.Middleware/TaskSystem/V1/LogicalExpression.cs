using System;
using System.Collections.Generic;

namespace PirateX.Middleware
{
    public class LogicalExpression
    {
        private readonly char and = '&';
        private readonly char or = '|';
        private readonly ILogical iLogical;

        public LogicalExpression(ILogical iLogical)
        {
            this.iLogical = iLogical ?? throw new ArgumentNullException("iLogical");
        }
        /// <summary>
        /// 逻辑表达式的构造函数
        /// 默认情况下逻辑与为&；逻辑或为|
        /// </summary>
        /// <param name="and">设置新的逻辑与</param>
        /// <param name="or">设置新的逻辑或</param>
        /// <param name="iLogical">单个项的逻辑处理接口实例,是实现的实例</param>
        public LogicalExpression(char and, char or, ILogical iLogical)
            : this(iLogical)
        {
            this.and = and;
            this.or = or;
        }
        /// <summary> 计算表达式的值 </summary>
        /// <param name="expression"> 表达式 例如(A|B|C)&(D|E|F)&G </param>
        /// <returns> 表达式的逻辑值TRUE | FALSE </returns>
        public bool Process(string expression)
        {
            return Process(false, expression);
        }

        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="sub"> 是否是子表达式,即，在括号内 </param>
        /// <param name="expression"> 表达式 </param>
        /// <returns> 计算得到的逻辑值 </returns>
        private bool Process(bool sub, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException("expression");
            char[] c = expression.ToCharArray();
            int length = c.Length;
            bool flag = false;
            bool start = false; //是否有过一次 与或计算
            bool mark = false; //读取括号中的内容
            bool logic = true; //true是& false是|
            Queue<char> queue = new Queue<char>();//存入数字
            bool temp = false; //最小表达式的值

            for (int i = 0; i < length; i++)
            {
                if (c[i].Equals('('))
                {
                    mark = true;
                    temp = true;
                }
                else if (c[i].Equals(')'))
                {
                    bool b = Process(true, new string(queue.ToArray()));
                    flag = GetLogicValue(start, logic, flag, b);

                    temp = GetLogicValue(false, logic, temp, b);

                    iLogical.ConditionBlock(new string(queue.ToArray()), temp,true);

                    start = true;

                    mark = false;
                    queue.Clear();
                }
                else if (c[i].Equals(or))//逻辑或
                {
                    if (mark)
                    {
                        queue.Enqueue(c[i]);
                        continue;
                    }
                    if (queue.Count != 0)
                    {
                        bool b = iLogical.ProcessItem(new string(queue.ToArray()),sub);
                        flag = GetLogicValue(start, logic, flag, b);
                        temp = GetLogicValue(false, logic, temp, b);

                        if (!sub)
                            iLogical.ConditionBlock(new string(queue.ToArray()), temp,false);
                    }
                    logic = false;
                    start = true;

                    queue.Clear();
                }
                else if (c[i].Equals(and))//逻辑与
                {
                    if (mark)
                    {
                        queue.Enqueue(c[i]);
                        continue;
                    }
                    if (queue.Count != 0)
                    {
                        bool b = iLogical.ProcessItem(new string(queue.ToArray()),sub);
                        flag = GetLogicValue(start, logic, flag, b);

                        temp = GetLogicValue(false, logic, temp, b);
                        if (!sub)
                            iLogical.ConditionBlock(new string(queue.ToArray()), temp,false);
                    }
                    logic = true;
                    start = true;
                    queue.Clear();
                }
                else //if( char.IsDigit(c[i]) )
                {
                    queue.Enqueue(c[i]);
                }
            }
            if (queue.Count != 0)
            {
                bool b = iLogical.ProcessItem(new string(queue.ToArray()),sub);
                flag = GetLogicValue(start, logic, flag, b);

                temp = GetLogicValue(false, logic, temp, b);

                if (!sub)
                    iLogical.ConditionBlock(new string(queue.ToArray()), temp,false);
            }

            return flag;
        }

        private static bool GetLogicValue(bool start, bool logic, bool op1, bool op2)
        {
            bool flag = false;
            if (start)
            {
                if (logic)
                    flag = op1 & op2;
                else
                    flag = op1 | op2;
            }
            else
                flag = op2;

            return flag;
        }
    }
    /// <summary> 单个项的计算接口 </summary>
    public interface ILogical
    {
        /// <summary> 计算单个项的逻辑值 </summary>
        /// <param name="itemSymbol">单个项的标识</param>
        /// <param name="inner">是否读取括号中的条件</param>
        /// <returns>处理得到的逻辑值</returns>
        bool ProcessItem(string itemSymbol,bool inner);
        /// <summary> 逻辑块 即 括号内的逻辑值 </summary>
        /// <param name="key">逻辑表达式</param>
        /// <param name="value">逻辑值</param>
        /// <param name="comp">是否是复杂表达</param>
        void ConditionBlock(string key, bool value,bool comp);
    }
}
