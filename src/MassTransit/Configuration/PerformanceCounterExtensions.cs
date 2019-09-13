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
namespace MassTransit
{
    using System;
    using Monitoring.Performance;
    using Monitoring.Performance.StatsD;

#if !NETCORE
    using Monitoring.Performance.Windows;
#endif


    public static class PerformanceCounterExtensions
    {
#if !NETCORE
        [Obsolete("This method was improperly named, use EnablePerformanceCounters instead")]
        public static void EnabledPerformanceCounters(this IBusFactoryConfigurator configurator)
        {
            configurator.EnableWindowsPerformanceCounters();
        }

        /// <summary>
        /// Enable performance counters on the bus to emit performance data for use by performance
        /// monitor.
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("This method is replaced by EnableWindowsPerformanceCounters")]
        public static void EnablePerformanceCounters(this IBusFactoryConfigurator configurator)
        {
            configurator.EnableWindowsPerformanceCounters();
        }

        public static void EnableWindowsPerformanceCounters(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new PerformanceCounterBusObserver(new WindowsCounterFactory());
            configurator.ConnectBusObserver(observer);
        }
#endif

        public static void EnableStatsdPerformanceCounters(this IBusFactoryConfigurator configurator, Action<StatsDConfiguration> action)
        {
            var statsDConfiguration = StatsDConfiguration.Defaults();
            action(statsDConfiguration);

            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new PerformanceCounterBusObserver(new StatsDCounterFactory(statsDConfiguration));
            configurator.ConnectBusObserver(observer);
        }
    }
}
