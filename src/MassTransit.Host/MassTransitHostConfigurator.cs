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
namespace MassTransit.Host
{
    using System;
    using System.Diagnostics;
    using Topshelf;
    using Topshelf.HostConfigurators;
    using Topshelf.Runtime;


    public class MassTransitHostConfigurator<T>
        where T : TopshelfServiceBootstrapper<T>
    {
        T _bootstrapper;

        public MassTransitHostConfigurator()
        {
            ServiceName = typeof(T).GetDisplayName();
            DisplayName = typeof(T).GetDisplayName();
            Description = typeof(T).GetServiceDescription();

            BootstrapperFactory = settings => (T)Activator.CreateInstance(typeof(T), settings);
        }

        public string ServiceName { private get; set; }
        public string DisplayName { private get; set; }
        public string Description { private get; set; }

        Func<HostSettings, T> BootstrapperFactory { get; }

        public void Configure(HostConfigurator configurator)
        {
            configurator.SetServiceName(ServiceName);
            configurator.SetDisplayName(DisplayName);
            configurator.SetDescription(Description);

            configurator.AfterInstall(() =>
            {
                VerifyEventLogSourceExists(ServiceName);

                // this will force the performance counters to register during service installation
                // making them created - of course using the InstallUtil stuff completely skips
                // this part of the install :(
                BusPerformanceCounters.Install();
            });

            configurator.Service(settings =>
            {
                _bootstrapper = BootstrapperFactory(settings);

                return _bootstrapper.GetService();
            },
                s => s.AfterStoppingService(() =>
                {
                    if (_bootstrapper != default(T))
                        _bootstrapper.Dispose();
                }));
        }

        static void VerifyEventLogSourceExists(string serviceName)
        {
            if (!EventLog.SourceExists(serviceName))
                EventLog.CreateEventSource(serviceName, "RapidTransit");
        }
    }
}