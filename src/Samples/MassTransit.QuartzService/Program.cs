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
namespace MassTransit.QuartzService
{
    using System.Diagnostics;
    using Configuration;
    using Monitoring.Performance.Windows;
    using Topshelf;
    using Topshelf.Logging;
    using Topshelf.Runtime;


    class Program
    {
        public static int Main()
        {
            Log4NetLogWriterFactory.Use("log4net.config");


            return (int)HostFactory.Run(x =>
            {
                x.AfterInstall(() =>
                {
                    VerifyEventLogSourceExists();

                    // this will force the performance counters to register during service installation
                    // making them created - of course using the InstallUtil stuff completely skips
                    // this part of the install :(
                    new WindowsPerformanceCounterInstaller().Install();
                });

                x.Service(CreateService);
            });
        }

        static void VerifyEventLogSourceExists()
        {
            if (!EventLog.SourceExists("MassTransit"))
                EventLog.CreateEventSource("MassTransit Quartz Service", "MassTransit");
        }

        static ScheduleMessageService CreateService(HostSettings arg)
        {
            var configurationProvider = new FileConfigurationProvider();

            return new ScheduleMessageService(configurationProvider);
        }
    }
}
