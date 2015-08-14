using System;

namespace GameServer.Core.Ex
{
     [AttributeUsage(AttributeTargets.Field)]
    public class PExceptionMessageAttribute:Attribute
    {
        private readonly string _message;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">返回信息</param>
        public PExceptionMessageAttribute(string message)
        {
            this._message = message;
        }
        /// <summary>
        /// 返回信息描述
        /// </summary>
        public string Message { get { return _message; } }
        /// <summary>
        /// 重写ToString方法，返回异常信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _message; 
        }
    }
}
