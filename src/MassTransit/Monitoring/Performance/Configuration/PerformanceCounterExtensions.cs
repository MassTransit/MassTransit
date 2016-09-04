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
    using BusConfigurators;
    using Monitoring.Performance;
    using Monitoring.Performance.Windows;


    public static class PerformanceCounterExtensions
    {
        [Obsolete("This method was improperly named, use EnablePerformanceCounters instead")]
        public static void EnabledPerformanceCounters(this IBusFactoryConfigurator configurator)
        {
            configurator.EnablePerformanceCounters<WindowsCounterFactory>();
        }

        /// <summary>
        /// Enable performance counters on the bus to emit performance data for use by performance
        /// monitor.
        /// </summary>
        /// <param name="configurator"></param>
        public static void EnablePerformanceCounters<TFactory>(this IBusFactoryConfigurator configurator)
            where TFactory : ICounterFactory, new()
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new PerformanceCounterBusFactorySpecification<TFactory>();
            configurator.AddBusFactorySpecification(specification);
        }
    }
}