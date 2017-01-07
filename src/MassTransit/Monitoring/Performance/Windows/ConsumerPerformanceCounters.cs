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
namespace MassTransit.Monitoring.Performance.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;


    public class ConsumerPerformanceCounters :
        BaseWindowsPerformanceCounters
    {
        ConsumerPerformanceCounters()
            : base(BuiltInCounters.Consumers.Category.Name, BuiltInCounters.Consumers.Category.Help)
        {
        }

        public static CounterCreationData ConsumeRate => Cached.Instance.Value.Data[0];
        public static CounterCreationData TotalMessages => Cached.Instance.Value.Data[1];
        public static CounterCreationData Duration => Cached.Instance.Value.Data[2];
        public static CounterCreationData DurationBase => Cached.Instance.Value.Data[3];
        public static CounterCreationData TotalFaults => Cached.Instance.Value.Data[4];
        public static CounterCreationData FaultPercentage => Cached.Instance.Value.Data[5];
        public static CounterCreationData FaultPercentageBase => Cached.Instance.Value.Data[6];

        public static void Install()
        {
            ConsumerPerformanceCounters value = Cached.Instance.Value;
        }

        protected override IEnumerable<CounterCreationData> GetCounterData()
        {
            yield return Convert(BuiltInCounters.Consumers.Counters.MessagesPerSecond, PerformanceCounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Consumers.Counters.TotalMessages, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Consumers.Counters.AverageDuration, PerformanceCounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Consumers.Counters.AverageDurationBase, PerformanceCounterType.AverageBase);
            yield return Convert(BuiltInCounters.Consumers.Counters.TotalFaults, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Consumers.Counters.FaultPercent, PerformanceCounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Consumers.Counters.FaultPercentBase, PerformanceCounterType.AverageBase);
        }

        static class Cached
        {
            internal static readonly Lazy<ConsumerPerformanceCounters> Instance = new Lazy<ConsumerPerformanceCounters>(() => new ConsumerPerformanceCounters());
        }
    }
}