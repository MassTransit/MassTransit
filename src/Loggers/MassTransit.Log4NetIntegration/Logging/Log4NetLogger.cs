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
    using System.IO;
    using MassTransit.Logging;
    using log4net;
    using log4net.Config;

    public class Log4NetLogger :
        ILogger
    {
        public MassTransit.Logging.ILog Get(string name)
        {
#if NETCORE
            var logger = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), name);            
#else
            var logger = LogManager.GetLogger(name);
#endif
            return new Log4NetLog(logger);
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }

        public static void Use()
        {
            Logger.UseLogger(new Log4NetLogger());
        }

        public static void Use(string file)
        {
            Logger.UseLogger(new Log4NetLogger());

#if NETCORE
            file = Path.Combine(AppContext.BaseDirectory, file);
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(file));
#else
            file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            XmlConfigurator.Configure(new FileInfo(file));
#endif
        }
    }
}