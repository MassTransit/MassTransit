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
namespace MassTransit.Log4NetIntegration.Logging
{
    using System;
    using System.Globalization;
    using MassTransit.Logging;

    public class Log4NetLog :
        ILog
    {
        readonly log4net.ILog _log;

        public Log4NetLog(log4net.ILog log)
        {
            _log = log;
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Debug(LogOutputProvider messageProvider)
        {
            if (!IsDebugEnabled)
                return;

            _log.Debug(messageProvider());
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.DebugFormat(provider, format, args);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void Info(LogOutputProvider messageProvider)
        {
            if (!IsInfoEnabled)
                return;

            _log.Info(messageProvider());
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void Warn(LogOutputProvider messageProvider)
        {
            if (!IsWarnEnabled)
                return;

            _log.Warn(messageProvider());
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.WarnFormat(provider, format, args);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Error(LogOutputProvider messageProvider)
        {
            if (!IsErrorEnabled)
                return;

            _log.Error(messageProvider());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void Fatal(LogOutputProvider messageProvider)
        {
            if (!IsFatalEnabled)
                return;

            _log.Fatal(messageProvider());
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.FatalFormat(provider, format, args);
        }

        public bool IsDebugEnabled
        {
            get { return _log.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _log.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _log.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _log.IsFatalEnabled; }
        }

        public void Log(LogLevel level, object obj)
        {
            if (level == LogLevel.Fatal)
                Fatal(obj);
            else if (level == LogLevel.Error)
                Error(obj);
            else if (level == LogLevel.Warn)
                Warn(obj);
            else if (level == LogLevel.Info)
                Info(obj);
            else if (level >= LogLevel.Debug)
                Debug(obj);
        }

        public void Log(LogLevel level, object obj, Exception exception)
        {
            if (level == LogLevel.Fatal)
                Fatal(obj, exception);
            else if (level == LogLevel.Error)
                Error(obj, exception);
            else if (level == LogLevel.Warn)
                Warn(obj, exception);
            else if (level == LogLevel.Info)
                Info(obj, exception);
            else if (level >= LogLevel.Debug)
                Debug(obj, exception);
        }

        public void Log(LogLevel level, LogOutputProvider messageProvider)
        {
            if (level == LogLevel.Fatal)
                Fatal(messageProvider);
            else if (level == LogLevel.Error)
                Error(messageProvider);
            else if (level == LogLevel.Warn)
                Warn(messageProvider);
            else if (level == LogLevel.Info)
                Info(messageProvider);
            else if (level >= LogLevel.Debug)
                Debug(messageProvider);
        }

        public void LogFormat(LogLevel level, string format, params object[] args)
        {
            if (level == LogLevel.Fatal)
                FatalFormat(CultureInfo.InvariantCulture, format, args);
            else if (level == LogLevel.Error)
                ErrorFormat(CultureInfo.InvariantCulture, format, args);
            else if (level == LogLevel.Warn)
                WarnFormat(CultureInfo.InvariantCulture, format, args);
            else if (level == LogLevel.Info)
                InfoFormat(CultureInfo.InvariantCulture, format, args);
            else if (level >= LogLevel.Debug)
                DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void LogFormat(LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (level == LogLevel.Fatal)
                FatalFormat(formatProvider, format, args);
            else if (level == LogLevel.Error)
                ErrorFormat(formatProvider, format, args);
            else if (level == LogLevel.Warn)
                WarnFormat(formatProvider, format, args);
            else if (level == LogLevel.Info)
                InfoFormat(formatProvider, format, args);
            else if (level >= LogLevel.Debug)
                DebugFormat(formatProvider, format, args);
        }
    }
}