// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SeriLogIntegration
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using Serilog.Events;


    public class SerilogLog : ILog
    {
        public const string ObjectLogTemplate = "{@Obj}";

        static readonly Dictionary<LogLevel, LogEventLevel> LogLevelTypeMap = new Dictionary<LogLevel, LogEventLevel>
        {
            {LogLevel.Fatal, LogEventLevel.Fatal},
            {LogLevel.Error, LogEventLevel.Error},
            {LogLevel.Warn, LogEventLevel.Warning},
            {LogLevel.Info, LogEventLevel.Information},
            {LogLevel.Debug, LogEventLevel.Debug},
            {LogLevel.All, LogEventLevel.Verbose}
        };

        readonly bool _demoteDebug;
        readonly Serilog.ILogger _logger;

        public SerilogLog(Serilog.ILogger logger, bool demoteDebug = false)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _demoteDebug = demoteDebug;
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Debug); }
        }

        public bool IsErrorEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Fatal); }
        }

        public bool IsInfoEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Information); }
        }

        public bool IsWarnEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Warning); }
        }

        public void Debug(object obj)
        {
            WriteSerilog(_demoteDebug ? LogEventLevel.Verbose : LogEventLevel.Debug, obj);
        }

        public void Debug(object obj, Exception exception)
        {
            WriteSerilog(_demoteDebug ? LogEventLevel.Verbose : LogEventLevel.Debug, exception, obj);
        }

        public void Debug(LogOutputProvider messageProvider)
        {
            WriteSerilog(_demoteDebug ? LogEventLevel.Verbose : LogEventLevel.Debug, messageProvider);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            WriteSerilog(_demoteDebug ? LogEventLevel.Verbose : LogEventLevel.Debug, format, args);
        }

        public void DebugFormat(string format, params object[] args)
        {
            WriteSerilog(_demoteDebug ? LogEventLevel.Verbose : LogEventLevel.Debug, format, args);
        }

        public void Error(object obj)
        {
            WriteSerilog(LogEventLevel.Error, obj);
        }

        public void Error(object obj, Exception exception)
        {
            WriteSerilog(LogEventLevel.Error, exception, obj);
        }

        public void Error(LogOutputProvider messageProvider)
        {
            WriteSerilog(LogEventLevel.Error, messageProvider);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Error, format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Error, format, args);
        }

        public void Fatal(object obj)
        {
            WriteSerilog(LogEventLevel.Fatal, obj);
        }

        public void Fatal(object obj, Exception exception)
        {
            WriteSerilog(LogEventLevel.Fatal, exception, obj);
        }

        public void Fatal(LogOutputProvider messageProvider)
        {
            WriteSerilog(LogEventLevel.Fatal, messageProvider);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Fatal, format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Fatal, format, args);
        }

        public void Info(object obj)
        {
            WriteSerilog(LogEventLevel.Information, obj);
        }

        public void Info(object obj, Exception exception)
        {
            WriteSerilog(LogEventLevel.Information, exception, obj);
        }

        public void Info(LogOutputProvider messageProvider)
        {
            WriteSerilog(LogEventLevel.Information, messageProvider);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Information, format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Information, format, args);
        }

        public void Log(LogLevel level, object obj)
        {
            WriteSerilog(GetSerilogLevel(level), obj);
        }

        public void Log(LogLevel level, object obj, Exception exception)
        {
            WriteSerilog(GetSerilogLevel(level), exception, obj);
        }

        public void Log(LogLevel level, LogOutputProvider messageProvider)
        {
            WriteSerilog(GetSerilogLevel(level), messageProvider);
        }

        public void LogFormat(LogLevel level,
            IFormatProvider formatProvider,
            string format,
            params object[] args)
        {
            WriteSerilog(GetSerilogLevel(level), format, args);
        }

        public void LogFormat(LogLevel level, string format, params object[] args)
        {
            WriteSerilog(GetSerilogLevel(level), format, args);
        }

        public void Warn(object obj)
        {
            WriteSerilog(LogEventLevel.Warning, obj);
        }

        public void Warn(object obj, Exception exception)
        {
            WriteSerilog(LogEventLevel.Warning, exception, obj);
        }

        public void Warn(LogOutputProvider messageProvider)
        {
            WriteSerilog(LogEventLevel.Warning, messageProvider);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Warning, format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            WriteSerilog(LogEventLevel.Warning, format, args);
        }

        LogEventLevel GetSerilogLevel(LogLevel level)
        {
            LogEventLevel logEventLevel;

            return LogLevelTypeMap.TryGetValue(level, out logEventLevel) ? logEventLevel : LogEventLevel.Information;
        }

        void WriteSerilog(LogEventLevel level, object obj)
        {
            if (obj is string)
            {
                _logger.Write(level, "{Message:l}", obj);
            }
            else
            {
                _logger.Write(level, ObjectLogTemplate, obj);
            }
        }

        void WriteSerilog(LogEventLevel level, Exception exception, object obj)
        {
            if (obj is string)
            {
                _logger.Write(level, exception, obj as string);
            }
            else
            {
                _logger.Write(level, exception, ObjectLogTemplate, obj);
            }
        }

        void WriteSerilog(LogEventLevel level, string messageTemplate, object[] objects)
        {
            _logger.Write(level, messageTemplate, objects);
        }

        void WriteSerilog(LogEventLevel level, LogOutputProvider logOutputProvider)
        {
            if (_logger.IsEnabled(level))
            {
                _logger.Write(level, ObjectLogTemplate, logOutputProvider());
            }
        }
    }
}