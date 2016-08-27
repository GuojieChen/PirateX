using System;
using System.Resources;

namespace PirateX
{
    /// <summary> 异常类型 </summary>
    [Serializable]
    public class PirateXException : System.Exception
    {
        public int Status { get; set; }

        public StatusCode StatusCode
        {
            get
            {
                return (StatusCode)this.Status;
            }
            set
            {
                this.Status = (int)value;
            }
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 附带的参数值列表
        /// </summary>
        public object[] Params { get; set; }

        public PirateXException(StatusCode status)
             : this(status, string.Empty, string.Empty, (Exception)null)
        {

        }

        public PirateXException(StatusCode status, string errorCode)
             : this(status, errorCode, string.Empty, (Exception)null)
        {
            
        }

        public PirateXException(StatusCode status, string errorCode, string errorMessage, Exception innerException) 
            :base(innerException.Message, innerException)
        {
            StatusCode = status;
            this.ErrorCode = errorCode ?? status.ToString();
            this.ErrorMessage = errorMessage;
        }

        public PirateXException(string errorCode, string errorMessage) 
            :this(StatusCode.Ok,errorCode,errorMessage, (Exception)null)
        {
        }

        
        /// <summary>
        /// [已过时，直接使用ToString 即可]
        /// 返回异常的描述，也就是Code所表示的异常信息。
        /// <p>如果项目配置了Resource文件，则会返回Resource中Code名称对应值。</p>
        /// <p>如果没有Resource文件对应的值，则查看Code的PirateErrorMessage值</p>
        /// <p>除此，直接返回Code的名称描述</p>
        /// </summary>
        /// <returns>异常描述信息</returns>
        public string ToDescription()
        {
            string des = GetEnumDescription(StatusCode);
            
            if (Params != null)
                return String.Format(des, Params);
            else
                return des;
        }
        /// <summary>
        /// 返回异常的描述，也就是Code所表示的异常信息。
        /// <p>如果项目配置了Resource文件，则会返回Resource中Code名称对应值。</p>
        /// <p>如果没有Resource文件对应的值，则查看Code的PirateErrorMessage值</p>
        /// <p>除此，直接返回Code的名称描述</p>
        /// </summary>
        /// <returns>异常描述信息</returns>
        public override string ToString()
        {
            string des = GetEnumDescription(StatusCode);

            if (Params != null)
                return String.Format(des, Params);
            else
                return des;
        }

        //public override string Message
        //{
        //    get { return ToString(); }
        //}

        /// <summary> 获取 错误
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeratedType"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(T enumeratedType)
        {
            var description = enumeratedType.ToString();

            ResourceManager rm = null;

            string des = null;
            try
            {
                rm = new ResourceManager($"{enumeratedType.GetType().Assembly.GetName().Name}.Resources.Resource", enumeratedType.GetType().Assembly);
                des = rm.GetString(description);
            }
            catch (System.Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Format("请在文件夹Reousrces下建立Resource.resx 文件"));
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (string.IsNullOrEmpty(des))
            {
                var fieldInfo = enumeratedType.GetType().GetField(description);

                if (fieldInfo != null)
                {
                    var attributes = fieldInfo.GetCustomAttributes(false);

                    if (attributes.Length > 0)
                    {
                        des = attributes[0].ToString();
                    }
                    else
                    {
                        des = description;
                    }
                }
                else
                    des = "{0}"; //= description; 
            }

            if (string.IsNullOrEmpty(des))
                des = "{0}";

            return des;
        }
    }
}
