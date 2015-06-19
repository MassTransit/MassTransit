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
namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security;
    using Logging;


    public abstract class PerformanceCounters
    {
        readonly string _categoryHelp;
        readonly string _categoryName;
        readonly Lazy<CounterCreationData[]> _counterCreationData;
        readonly CreateCounterDelegate _createCounter;
        readonly ILog _log = Logger.Get<PerformanceCounters>();

        protected PerformanceCounters(string categoryName, string categoryHelp)
        {
            _categoryName = categoryName;
            _categoryHelp = categoryHelp;
            _counterCreationData = new Lazy<CounterCreationData[]>(() => GetCounterData().ToArray());

            _createCounter = Initialize();
        }

        protected CounterCreationData[] Data
        {
            get { return _counterCreationData.Value; }
        }

        protected IPerformanceCounter CreatePerformanceCounter(string counterName, string instanceName)
        {
            return _createCounter(counterName, instanceName);
        }

        IPerformanceCounter PerformanceCounterFactory(string counterName, string instanceName)
        {
            var counter = new PerformanceCounter(_categoryName, counterName, instanceName, false);
            return new InstancePerformanceCounter(counter);
        }

        IPerformanceCounter NullCounterFactory(string counterName, string consumerType)
        {
            return new NullPerformanceCounter();
        }

        CreateCounterDelegate Initialize()
        {
            try
            {
                CounterCreationData[] counters = Data;

                if (!PerformanceCounterCategory.Exists(_categoryName))
                {
                    CreateCategory(counters);

                    return PerformanceCounterFactory;
                }

                IEnumerable<CounterCreationData> missing =
                    counters.Where(counter => !PerformanceCounterCategory.CounterExists(counter.CounterName, _categoryName));
                if (missing.Any())
                {
                    PerformanceCounterCategory.Delete(_categoryName);

                    CreateCategory(counters);
                }

                return PerformanceCounterFactory;
            }
            catch (SecurityException)
            {
                _log.WarnFormat(
                    "Unable to create performance counter category (Category: {0})" +
                        "\nTry running the program in the Administrator role to set these up." +
                        "\n**Hey, this just means you aren't admin or don't have/want perf counter support**", _categoryName);

                return NullCounterFactory;
            }
        }

        protected abstract IEnumerable<CounterCreationData> GetCounterData();

        void CreateCategory(CounterCreationData[] counters)
        {
            PerformanceCounterCategory.Create(
                _categoryName,
                _categoryHelp,
                PerformanceCounterCategoryType.MultiInstance,
                new CounterCreationDataCollection(counters));
        }


        delegate IPerformanceCounter CreateCounterDelegate(string counterName, string consumerType);
    }
}