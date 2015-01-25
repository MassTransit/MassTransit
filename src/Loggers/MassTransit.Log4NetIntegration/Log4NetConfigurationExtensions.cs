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
namespace MassTransit.Log4NetIntegration
{
    using System;
    using System.IO;
    using Logging;


    /// <summary>
    /// Extensions for configuring MassTransit for log4net
    /// </summary>
    public static class Log4NetConfigurationExtensions
    {
        /// <summary>
        /// Specify that you want to use the Log4net logging engine for logging with MassTransit.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseLog4Net(this IBusFactoryConfigurator configurator)
        {
            Log4NetLogger.Use();
        }

        /// <summary>
        /// Specify that log4net should be used for logging MassTransit messages
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configFileName">The name of the log4net xml configuration file</param>
        public static void UseLog4Net(this IBusFactoryConfigurator configurator, string configFileName)
        {
            Log4NetLogger.Use();

            string path = AppDomain.CurrentDomain.BaseDirectory;

            string file = Path.Combine(path, configFileName);

            var configFile = new FileInfo(file);
            if (configFile.Exists)
                Log4NetLogger.Use(file);
        }
    }
}