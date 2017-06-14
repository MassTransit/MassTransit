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
namespace MassTransit.ExtensionsLoggingIntegration
{
    using System;
    using MT = Logging;
    using Microsoft.Extensions.Logging;

    public class ExtensionsLog : MT.ILog
    {
        private readonly ILogger _logger;

        public ExtensionsLog(ILogger logger)
        {
            _logger = logger;            
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsEnabled(LogLevel.Debug); }
        }

        public bool IsErrorEnabled
        {
            get { return _logger.IsEnabled(LogLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return _logger.IsEnabled(LogLevel.Critical); }
        }

        public bool IsInfoEnabled
        {
            get { return _logger.IsEnabled(LogLevel.Information); }
        }

        public bool IsWarnEnabled
        {
            get { return _logger.IsEnabled(LogLevel.Warning); }
        }

        public void Debug(MT.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Debug, 0, messageProvider, null, MessageFormatter);
        }

        public void Debug(object obj)
        {
            _logger.LogDebug(obj?.ToString());
        }

        public void Debug(object obj, Exception exception)
        {
            _logger.LogDebug(0, exception, obj?.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.LogDebug(format, args);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogDebug(string.Format(formatProvider, format, args));
        }

        public void Error(MT.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Error, 0, messageProvider, null, MessageFormatter);
        }

        public void Error(object obj)
        {
            _logger.LogError(obj?.ToString());
        }

        public void Error(object obj, Exception exception)
        {
            _logger.LogError(0, exception, obj?.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.LogError(format, args);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogError(string.Format(formatProvider, format, args));
        }

        public void Fatal(MT.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Critical, 0, messageProvider, null, MessageFormatter);
        }

        public void Fatal(object obj)
        {
            _logger.LogCritical(obj?.ToString());
        }

        public void Fatal(object obj, Exception exception)
        {
            _logger.LogCritical(0, exception, obj?.ToString());
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.LogCritical(format, args);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogCritical(string.Format(formatProvider, format, args));
        }

        public void Info(MT.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Information, 0, messageProvider, null, MessageFormatter);
        }

        public void Info(object obj)
        {
            _logger.LogInformation(obj?.ToString());
        }

        public void Info(object obj, Exception exception)
        {
            _logger.LogInformation(0, exception, obj?.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.LogInformation(format, args);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogInformation(string.Format(formatProvider, format, args));
        }

        public void LogFormat(LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(MT.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Warning, 0, messageProvider, null, MessageFormatter);
        }

        public void Warn(object obj)
        {
            _logger.LogWarning(obj?.ToString());
        }

        public void Warn(object obj, Exception exception)
        {
            _logger.LogWarning(0, exception, obj?.ToString());
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.LogWarning(format, args);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogWarning(string.Format(formatProvider, format, args));
        }

        public void Log(MT.LogLevel level, object obj)
        {
            if (level == MT.LogLevel.Fatal)
                Fatal(obj);
            else if (level == MT.LogLevel.Error)
                Error(obj);
            else if (level == MT.LogLevel.Warn)
                Warn(obj);
            else if (level == MT.LogLevel.Info)
                Info(obj);
            else if (level >= MT.LogLevel.Debug)
                Debug(obj);
        }

        public void Log(MT.LogLevel level, object obj, Exception exception)
        {
            if (level == MT.LogLevel.Fatal)
                Fatal(obj, exception);
            else if (level == MT.LogLevel.Error)
                Error(obj, exception);
            else if (level == MT.LogLevel.Warn)
                Warn(obj, exception);
            else if (level == MT.LogLevel.Info)
                Info(obj, exception);
            else if (level >= MT.LogLevel.Debug)
                Debug(obj, exception);
        }

        public void Log(MT.LogLevel level, MT.LogOutputProvider messageProvider)
        {
            if (level == MT.LogLevel.Fatal)
                Fatal(messageProvider);
            else if (level == MT.LogLevel.Error)
                Error(messageProvider);
            else if (level == MT.LogLevel.Warn)
                Warn(messageProvider);
            else if (level == MT.LogLevel.Info)
                Info(messageProvider);
            else if (level >= MT.LogLevel.Debug)
                Debug(messageProvider);
        }

        public void LogFormat(MT.LogLevel level, string format, params object[] args)
        {
            if (level == MT.LogLevel.Fatal)
                FatalFormat(format, args);
            else if (level == MT.LogLevel.Error)
                ErrorFormat(format, args);
            else if (level == MT.LogLevel.Warn)
                WarnFormat(format, args);
            else if (level == MT.LogLevel.Info)
                InfoFormat(format, args);
            else if (level >= MT.LogLevel.Debug)
                DebugFormat(format, args);
        }

        public void LogFormat(MT.LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (level == MT.LogLevel.Fatal)
                FatalFormat(formatProvider, format, args);
            else if (level == MT.LogLevel.Error)
                ErrorFormat(formatProvider, format, args);
            else if (level == MT.LogLevel.Warn)
                WarnFormat(formatProvider, format, args);
            else if (level == MT.LogLevel.Info)
                InfoFormat(formatProvider, format, args);
            else if (level >= MT.LogLevel.Debug)
                DebugFormat(formatProvider, format, args);
        }

        private static string MessageFormatter(MT.LogOutputProvider output, Exception error)
        {
            return output == null ? null : output()?.ToString();
        }
    }
}