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
    using Microsoft.Extensions.Logging;

    public class ExtensionsLogger : Logging.ILogger
    {
        readonly ILoggerFactory _loggerFactory;

        public ExtensionsLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Logging.ILog Get(string name)
        {
            return new ExtensionsLog(_loggerFactory.CreateLogger(name));
        }

        public void Shutdown()
        {
            _loggerFactory.Dispose();
        }

        /// <summary>
        /// Use Microsoft.Extensions.Logging for logging in MassTransit. If no LoggerFactory is provided,
        /// one will be created.
        /// </summary>
        /// <param name="loggerFactory">The (optional) logger factory.</param>
        public static ILoggerFactory Use(ILoggerFactory loggerFactory = null)
        {
            loggerFactory = loggerFactory ?? new LoggerFactory();
            Logging.Logger.UseLogger(new ExtensionsLogger(loggerFactory));
            return loggerFactory;
        }
    }
}