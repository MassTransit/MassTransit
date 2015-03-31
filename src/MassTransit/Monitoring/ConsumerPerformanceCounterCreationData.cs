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
namespace MassTransit.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;


    public class ConsumerPerformanceCounterCreationData
    {
        readonly Lazy<CounterCreationData[]> _counterCreationData;

        public const string CategoryName = "MassTransit Consumers";

        public ConsumerPerformanceCounterCreationData()
        {
            _counterCreationData = new Lazy<CounterCreationData[]>(() => GetCounterData().ToArray());
        }

        public CounterCreationData[] Data
        {
            get { return _counterCreationData.Value; }
        }

        public static CounterCreationData ConsumedPerSecond
        {
            get { return Cached.Instance.Data[0]; }
        }

        public static CounterCreationData TotalReceived
        {
            get { return Cached.Instance.Data[1]; }
        }

        public static CounterCreationData ConsumeDuration
        {
            get { return Cached.Instance.Data[2]; }
        }

        public static CounterCreationData ConsumeDurationBase
        {
            get { return Cached.Instance.Data[3]; }
        }

        public IEnumerable<CounterCreationData> GetCounterData()
        {
            yield return
                new CounterCreationData("Messages/s", "Number of messages consumed per second", PerformanceCounterType.RateOfCountsPerSecond32);
            yield return
                new CounterCreationData("Total Consumed", "Total number of messages consumed", PerformanceCounterType.NumberOfItems64);

            yield return
                new CounterCreationData("Average Consumer Duration", "The average time spent consuming a message", PerformanceCounterType.AverageCount64);
            yield return
                new CounterCreationData("Average Consumer Duration Base", "The average time spent consuming a message", PerformanceCounterType.AverageBase);
        }


        class Cached
        {
            internal static readonly ConsumerPerformanceCounterCreationData Instance = new ConsumerPerformanceCounterCreationData();
        }
    }
}