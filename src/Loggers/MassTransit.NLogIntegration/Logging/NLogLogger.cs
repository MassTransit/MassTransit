// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Concurrent;
    using MassTransit.Logging;
    using NLog;


    public class NLogLogger :
        MassTransit.Logging.ILogger
    {
        readonly Func<string, NLog.Logger> _logFactory;
        readonly ConcurrentDictionary<string, NLogLog> _logs; 

        public NLogLogger(LogFactory factory)
        {
            _logFactory = factory.GetLogger;
            _logs = new ConcurrentDictionary<string, NLogLog>();
        }

        public NLogLogger()
        {
            _logFactory = LogManager.GetLogger;
        }

        public ILog Get(string name)
        {
            return _logs.GetOrAdd(name, x => new NLogLog(_logFactory(x), x));
        }

        public void Shutdown()
        {
            foreach (var log in _logs.Values)
            {
                log.Shutdown();
            }
        }

        public static void Use()
        {
            MassTransit.Logging.Logger.UseLogger(new NLogLogger());
        }
    }
}