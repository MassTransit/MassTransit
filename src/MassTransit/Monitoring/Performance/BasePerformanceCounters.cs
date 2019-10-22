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
    using Context;


    public abstract class BasePerformanceCounters
    {
        readonly Lazy<CounterData[]> _counterCreationData;

#if !NETCORE
        readonly string _categoryHelp;
        readonly string _categoryName;
#endif

#if !NETCORE
        protected BasePerformanceCounters(string categoryName, string categoryHelp)
        {
            _categoryName = categoryName;
            _categoryHelp = categoryHelp;
            _counterCreationData = new Lazy<CounterData[]>(() => GetCounterData().ToArray());

            Initialize();
        }
#else
        protected BasePerformanceCounters(){
            _counterCreationData = new Lazy<CounterData[]>(() => GetCounterData().ToArray());
            Initialize();
        }
#endif

        protected CounterData[] Data => _counterCreationData.Value;

        void Initialize()
        {
#if !NETCORE
            try
            {
                var counters = Data.Select(x => x.ToCounterCreationData());

                if (!PerformanceCounterCategory.Exists(_categoryName))
                {
                    CreateCategory(counters);

                    return;
                }

                IEnumerable<CounterCreationData> missing =
                    counters.Where(counter => !PerformanceCounterCategory.CounterExists(counter.CounterName, _categoryName));

                if (missing.Any())
                {
                    PerformanceCounterCategory.Delete(_categoryName);

                    CreateCategory(counters);
                }
            }
            catch (SecurityException exception)
            {
                LogContext.Warning?.Log(exception, "Unable to create performance counter category (Category: {Category})" +
                    "\nTry running the program in the Administrator role to set these up." +
                    "\n**Hey, this just means you aren't admin or don't have/want perf counter support**", _categoryName);
            }
#endif
        }

        protected abstract IEnumerable<CounterData> GetCounterData();

#if !NETCORE
        void CreateCategory(IEnumerable<CounterCreationData> counters)
        {
            PerformanceCounterCategory.Create(
                _categoryName,
                _categoryHelp,
                PerformanceCounterCategoryType.MultiInstance,
                new CounterCreationDataCollection(counters.ToArray()));
        }
#endif

        protected CounterData Convert(Counter counter, CounterType type)
        {
            return new CounterData
            {
                CounterName = counter.Name,
                CounterDescription = counter.Help,
                CounterType = type
            };
        }
    }
}
