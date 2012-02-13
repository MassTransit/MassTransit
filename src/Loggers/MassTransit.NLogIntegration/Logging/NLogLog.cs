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
namespace MassTransit.NLogIntegration.Logging
{
    using System;
    using MassTransit.Logging;
    using NLog;
    using Util;

    /// <summary>
    /// A logger that wraps to NLog. See http://stackoverflow.com/questions/7412156/how-to-retain-callsite-information-when-wrapping-nlog
    /// </summary>
    public class NLogLog :
        ILog
    {
        readonly NLog.Logger _log;

        /// <summary>
        /// Create a new NLog logger instance.
        /// </summary>
        /// <param name="name">Name of type to log as.</param>
        public NLogLog([NotNull] NLog.Logger log, [NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            _log = log;
            _log.Debug(() => "");
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

        public void Debug(object obj)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Debug, _log.Name, null, "{0}", new[] {obj}));
        }

        public void Debug(object obj, Exception exception)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Debug, _log.Name, null, "{0}", new[] {obj}, exception));
        }

        public void Info(object obj)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Info, _log.Name, null, "{0}", new[] {obj}));
        }

        public void Info(object obj, Exception exception)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Info, _log.Name, null, "{0}", new[] {obj}, exception));
        }

        public void Warn(object obj)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Warn, _log.Name, null, "{0}", new[] {obj}));
        }

        public void Warn(object obj, Exception exception)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Warn, _log.Name, null, "{0}", new[] {obj}, exception));
        }

        public void Error(object obj)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Error, _log.Name, null, "{0}", new[] {obj}));
        }

        public void Error(object obj, Exception exception)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Error, _log.Name, null, "{0}", new[] {obj}, exception));
        }

        public void Fatal(object obj)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Fatal, _log.Name, null, "{0}", new[] {obj}));
        }

        public void Fatal(object obj, Exception exception)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Fatal, _log.Name, null, "{0}", new[] {obj}, exception));
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Debug, _log.Name, formatProvider, format, args));
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Debug, _log.Name, null, format, args));
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Info, _log.Name, formatProvider, format, args));
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Info, _log.Name, null, format, args));
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Warn, _log.Name, formatProvider, format, args));
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Warn, _log.Name, null, format, args));
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Error, _log.Name, formatProvider, format, args));
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Error, _log.Name, null, format, args));
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Fatal, _log.Name, formatProvider, format, args));
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.Log(typeof (NLogLog), new LogEventInfo(LogLevel.Fatal, _log.Name, null, format, args));
        }
    }
}