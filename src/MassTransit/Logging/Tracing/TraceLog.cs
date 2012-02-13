// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Logging.Tracing
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    public class TraceLog :
        ILog
    {
        readonly string _name;
        readonly TraceSource _source;
        LogLevel _level;

        public TraceLog(string name, TraceSource source)
        {
            _name = name;
            _source = source;
            _level = LogLevel.None;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsDebugEnabled
        {
            get { return _level >= LogLevel.Debug; }
        }

        public bool IsInfoEnabled
        {
            get { return _level >= LogLevel.Info; }
        }

        public bool IsWarnEnabled
        {
            get { return _level >= LogLevel.Warn; }
        }

        public bool IsErrorEnabled
        {
            get { return _level >= LogLevel.Error; }
        }

        public bool IsFatalEnabled
        {
            get { return _level >= LogLevel.Fatal; }
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

    	public void Debug(LogMessageGenerator messageGenerator)
    	{
			if (IsDebugEnabled)
				Log(LogLevel.Debug, messageGenerator(), null);
    	}

    	/// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        public void Debug(object message, Exception exception)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void DebugFormat(string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, string.Format(formatProvider, format, args), null);
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

    	public void Info(LogMessageGenerator messageGenerator)
    	{
			if (IsInfoEnabled)
				Log(LogLevel.Info, messageGenerator(), null);
    	}

    	/// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        public void Info(object message, Exception exception)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, message, exception);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void InfoFormat(string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, string.Format(formatProvider, format, args), null);
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

    	public void Warn(LogMessageGenerator messageGenerator)
    	{
    		throw new NotImplementedException();
    	}

    	/// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        public void Warn(object message, Exception exception)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, message, exception);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void WarnFormat(string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, string.Format(formatProvider, format, args), null);
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

    	public void Error(LogMessageGenerator messageGenerator)
    	{
    		if (IsErrorEnabled)
				Log(LogLevel.Error, messageGenerator() , null);
    	}

    	/// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        public void Error(object message, Exception exception)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, string.Format(formatProvider, format, args), null);
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

    	public void Fatal(LogMessageGenerator messageGenerator)
    	{
			if (IsFatalEnabled)
				Log(LogLevel.Fatal, messageGenerator(), null);
    	}

    	/// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        public void Fatal(object message, Exception exception)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void FatalFormat(string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, string.Format(formatProvider, format, args), null);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs a debug message.
        /// 
        /// </summary>
        /// <param name="format">Message format</param><param name="args">Array of objects to write using format</param>
        public void Debug(string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            Log(LogLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="format">Message format</param><param name="args">Array of objects to write using format</param>
        public void Info(string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            Log(LogLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="format">Message format</param><param name="args">Array of objects to write using format</param>
        public void Warn(string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            Log(LogLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="format">Message format</param><param name="args">Array of objects to write using format</param>
        public void Error(string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            Log(LogLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="formatProvider">The format provider to use</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, string.Format(formatProvider, format, args), exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="format">Message format</param><param name="args">Array of objects to write using format</param>
        public void Fatal(string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            Log(LogLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), null);
        }

        void Log(LogLevel level, object obj, Exception exception)
        {
            Log(level, obj == null ? "" : obj.ToString(), exception);
        }

        void Log(LogLevel level, string message, Exception exception)
        {
            if (exception == null)
                _source.TraceEvent(level.TraceEventType, 0, message);
            else
                _source.TraceData(level.TraceEventType, 0, (object) message, (object) exception);
        }
    }
}