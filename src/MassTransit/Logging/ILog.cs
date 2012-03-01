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
namespace MassTransit.Logging
{
    using System;

    /// <summary>
    /// Implementers handle logging and filtering based on logging levels.
    /// </summary>
    public interface ILog
    {
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

        void Log(LogLevel level, object obj);
        void Log(LogLevel level, object obj, Exception exception);
        void Log(LogLevel level, LogOutputProvider messageProvider);
        void LogFormat(LogLevel level, IFormatProvider formatProvider, string format, params object[] args);
        void LogFormat(LogLevel level, string format, params object[] args);

        void Debug(object obj);
        void Debug(object obj, Exception exception);
        void Debug(LogOutputProvider messageProvider);
        void DebugFormat(IFormatProvider formatProvider, string format, params object[] args);
        void DebugFormat(string format, params object[] args);

        void Info(object obj);
        void Info(object obj, Exception exception);
        void Info(LogOutputProvider messageProvider);
        void InfoFormat(IFormatProvider formatProvider, string format, params object[] args);
        void InfoFormat(string format, params object[] args);

        void Warn(object obj);
        void Warn(object obj, Exception exception);
        void Warn(LogOutputProvider messageProvider);
        void WarnFormat(IFormatProvider formatProvider, string format, params object[] args);
        void WarnFormat(string format, params object[] args);

        void Error(object obj);
        void Error(object obj, Exception exception);
        void Error(LogOutputProvider messageProvider);
        void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args);
        void ErrorFormat(string format, params object[] args);

        void Fatal(object obj);
        void Fatal(object obj, Exception exception);
        void Fatal(LogOutputProvider messageProvider);
        void FatalFormat(IFormatProvider formatProvider, string format, params object[] args);
        void FatalFormat(string format, params object[] args);
    }
}