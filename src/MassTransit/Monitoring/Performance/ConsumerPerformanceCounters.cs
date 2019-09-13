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


    public class ConsumerPerformanceCounters :
        BasePerformanceCounters
    {

#if !NETCORE
        ConsumerPerformanceCounters()
            : base(BuiltInCounters.Consumers.Category.Name, BuiltInCounters.Consumers.Category.Help)
        {
        }
#endif

        public static CounterData ConsumeRate => Cached.Instance.Value.Data[0];
        public static CounterData TotalMessages => Cached.Instance.Value.Data[1];
        public static CounterData Duration => Cached.Instance.Value.Data[2];
        public static CounterData DurationBase => Cached.Instance.Value.Data[3];
        public static CounterData TotalFaults => Cached.Instance.Value.Data[4];
        public static CounterData FaultPercentage => Cached.Instance.Value.Data[5];
        public static CounterData FaultPercentageBase => Cached.Instance.Value.Data[6];

        public static void Install()
        {
            _ = Cached.Instance.Value;
        }

        protected override IEnumerable<CounterData> GetCounterData()
        {
            yield return Convert(BuiltInCounters.Consumers.Counters.MessagesPerSecond, CounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Consumers.Counters.TotalMessages, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Consumers.Counters.AverageDuration,CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Consumers.Counters.AverageDurationBase, CounterType.AverageBase);
            yield return Convert(BuiltInCounters.Consumers.Counters.TotalFaults, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Consumers.Counters.FaultPercent, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Consumers.Counters.FaultPercentBase, CounterType.AverageBase);
        }

        static class Cached
        {
            internal static readonly Lazy<ConsumerPerformanceCounters> Instance = new Lazy<ConsumerPerformanceCounters>(() => new ConsumerPerformanceCounters());
        }
    }
}
