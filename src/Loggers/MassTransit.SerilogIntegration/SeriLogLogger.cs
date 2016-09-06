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
    using System.Collections.Concurrent;
    using Logging;
    using Serilog;
    using Serilog.Core;


    public class SerilogLogger :
        Logging.ILogger
    {
        readonly Serilog.ILogger _baseLogger;

        readonly bool _demoteDebug;

        readonly ConcurrentDictionary<string, ILog> _logs;

        public SerilogLogger(Serilog.ILogger baseLogger = null, bool demoteDebug = false)
        {
            _baseLogger = baseLogger;
            _demoteDebug = demoteDebug;
            _logs = new ConcurrentDictionary<string, ILog>();
        }

        public ILog Get(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _logs.GetOrAdd(name, CreateLog);
        }

        public void Shutdown()
        {
        }

        ILog CreateLog(string name)
        {
            var logger = _baseLogger ?? Log.Logger;
            if (logger == null)
            {
                throw new ArgumentException("An valid instance of Serilog was not available.");
            }

            return new SerilogLog(logger.ForContext(Constants.SourceContextPropertyName, name), _demoteDebug);
        }

        /// <summary>
        /// Use Serilog for logging in Mass Transit. If no Serilog ILogger instance is provided,
        /// use the Log.Logger global instance.
        /// </summary>
        /// <param name="baseLogger"></param>
        /// <param name="demoteDebug"></param>
        public static void Use(Serilog.ILogger baseLogger = null, bool demoteDebug = false)
        {
            Logging.Logger.UseLogger(new SerilogLogger(baseLogger, demoteDebug));
        }
    }
}