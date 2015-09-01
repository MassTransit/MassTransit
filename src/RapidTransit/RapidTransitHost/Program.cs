// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace RapidTransit
{
    using System;
    using System.IO;
    using log4net.Config;
    using Topshelf;
    using Topshelf.Logging;


    class Program
    {
        static int Main()
        {
            SetupLogger();

            return (int)HostFactory.Run(x =>
            {
                var configurator = new RapidTransitHostConfigurator<HostServiceBootstrapper>
                {
                    Description = "RapidTransit - A MassTransit Bus Host",
                    DisplayName = "RapidTransit MassTransit Host",
                    ServiceName = "rthost",
                };
                configurator.Configure(x);
            });
        }

        static void SetupLogger()
        {
            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RapidTransitHost.log4net.config"));
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);

            Log4NetLogWriterFactory.Use();
        }
    }
}