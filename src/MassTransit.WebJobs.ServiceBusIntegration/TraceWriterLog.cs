// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Logging;
    using Microsoft.Azure.WebJobs.Host;


    public class TraceWriterLog :
        ILog
    {
        readonly TraceLevel _level;
        readonly TraceWriter _writer;

        public TraceWriterLog(TraceWriter writer)
        {
            _writer = writer;
            _level = writer.Level;
        }

        public bool IsDebugEnabled => _level >= TraceLevel.Verbose;

        public bool IsInfoEnabled => _level >= TraceLevel.Info;

        public bool IsWarnEnabled => _level >= TraceLevel.Warning;

        public bool IsErrorEnabled => _level >= TraceLevel.Error;

        public bool IsFatalEnabled => _level >= TraceLevel.Error;

        public void LogFormat(LogLevel level, string format, params object[] args)
        {
            if (_level < level)
                return;

            LogInternal(level, string.Format(format, args), null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Debug(object message)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, message, null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        public void Debug(object message, Exception exception)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, message, exception);
        }

        public void Debug(LogOutputProvider messageProvider)
        {
            if (!IsDebugEnabled)
                return;

            object obj = messageProvider();

            LogInternal(TraceLevel.Verbose, obj, null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void DebugFormat(string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            LogInternal(TraceLevel.Verbose, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            LogInternal(TraceLevel.Verbose, string.Format(formatProvider, format, args), null);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Info(object message)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, message, null);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        public void Info(object message, Exception exception)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, message, exception);
        }

        public void Info(LogOutputProvider messageProvider)
        {
            if (!IsInfoEnabled)
                return;

            object obj = messageProvider();

            LogInternal(TraceLevel.Info, obj, null);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void InfoFormat(string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            LogInternal(TraceLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            LogInternal(TraceLevel.Info, string.Format(formatProvider, format, args), null);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Warn(object message)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, message, null);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        public void Warn(object message, Exception exception)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, message, exception);
        }

        public void Warn(LogOutputProvider messageProvider)
        {
            if (!IsWarnEnabled)
                return;

            object obj = messageProvider();

            LogInternal(TraceLevel.Warning, obj, null);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void WarnFormat(string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            LogInternal(TraceLevel.Warning, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            LogInternal(TraceLevel.Warning, string.Format(formatProvider, format, args), null);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Error(object message)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, message, null);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        public void Error(object message, Exception exception)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, message, exception);
        }

        public void Error(LogOutputProvider messageProvider)
        {
            if (!IsErrorEnabled)
                return;

            object obj = messageProvider();

            LogInternal(TraceLevel.Error, obj, null);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(formatProvider, format, args), null);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Fatal(object message)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, message, null);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        public void Fatal(object message, Exception exception)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, message, exception);
        }

        public void Fatal(LogOutputProvider messageProvider)
        {
            if (!IsFatalEnabled)
                return;

            object obj = messageProvider();

            LogInternal(TraceLevel.Error, obj, null);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void FatalFormat(string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(formatProvider, format, args), null);
        }

        public void Log(LogLevel level, object obj)
        {
            if (_level < level)
                return;

            LogInternal(level, obj, null);
        }

        public void Log(LogLevel level, object obj, Exception exception)
        {
            if (_level < level)
                return;

            LogInternal(level, obj, exception);
        }

        public void Log(LogLevel level, LogOutputProvider messageProvider)
        {
            if (_level < level)
                return;

            object obj = messageProvider();

            LogInternal(level, obj, null);
        }

        public void LogFormat(LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (_level < level)
                return;

            LogInternal(level, string.Format(formatProvider, format, args), null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            LogInternal(TraceLevel.Verbose, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            LogInternal(TraceLevel.Verbose, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            LogInternal(TraceLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            LogInternal(TraceLevel.Info, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            LogInternal(TraceLevel.Warning, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            LogInternal(TraceLevel.Warning, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            LogInternal(TraceLevel.Error, string.Format(formatProvider, format, args), exception);
        }

        void LogInternal(TraceLevel level, object obj, Exception exception)
        {
            string message = obj?.ToString() ?? "";

            var traceEvent = new TraceEvent(level, message, exception: exception);
            _writer.Trace(traceEvent);
        }
    }
}