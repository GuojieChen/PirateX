using System;
using NLog;
using SuperSocket.SocketBase.Logging;

namespace PirateX.Logging
{
    public class NLogLog : ILog
    {
        private readonly Logger m_Log;

        public NLogLog(Logger log)
        {

            if (log == null)
                throw new ArgumentNullException(nameof(log));

            m_Log = log;
        }


        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled
        {
            get { return m_Log.IsDebugEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is error enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is error enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsErrorEnabled
        {
            get { return m_Log.IsErrorEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fatal enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fatal enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFatalEnabled
        {
            get { return m_Log.IsFatalEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is info enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is info enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInfoEnabled
        {
            get { return m_Log.IsInfoEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is warn enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarnEnabled
        {
            get { return m_Log.IsWarnEnabled; }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            m_Log.Debug(message);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            if (message != null) m_Log.Debug(exception);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void DebugFormat(string format, object arg0)
        {
            m_Log.Debug(format, arg0);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            m_Log.Debug(format, args);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.Debug(provider, format, args);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void DebugFormat(string format, object arg0, object arg1)
        {
            m_Log.Debug(format, arg0, arg1);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.Debug(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            m_Log.Error(message);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            if (message != null) m_Log.Error(exception);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void ErrorFormat(string format, object arg0)
        {
            m_Log.Error(format, arg0);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            m_Log.Error(format, args);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.Error(provider, format, args);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            m_Log.Error(format, arg0, arg1);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.Error(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            m_Log.Fatal(message);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            m_Log?.Fatal(exception);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void FatalFormat(string format, object arg0)
        {
            m_Log.Fatal(format, arg0);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            m_Log.Fatal(format, args);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.Fatal(provider, format, args);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void FatalFormat(string format, object arg0, object arg1)
        {
            m_Log.Fatal(format, arg0, arg1);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.Fatal(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            m_Log.Info(message);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            m_Log.Info( exception);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void InfoFormat(string format, object arg0)
        {
            m_Log.Info(format, arg0);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            m_Log.Info(format, args);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.Info(provider, format, args);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void InfoFormat(string format, object arg0, object arg1)
        {
            m_Log.Info(format, arg0, arg1);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.Info(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            m_Log.Warn(message);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            m_Log.Warn(exception);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void WarnFormat(string format, object arg0)
        {
            m_Log.Warn(format, arg0);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            m_Log.Warn(format, args);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.Warn(provider, format, args);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void WarnFormat(string format, object arg0, object arg1)
        {
            m_Log.Warn(format, arg0, arg1);
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.Warn(format, arg0, arg1, arg2);
        }
    }
}
