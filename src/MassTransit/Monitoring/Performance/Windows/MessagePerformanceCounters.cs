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


    public class MessagePerformanceCounters :
        BaseWindowsPerformanceCounters
    {
        MessagePerformanceCounters()
            : base(BuiltInCounters.Messages.Category.Name, BuiltInCounters.Messages.Category.Help)
        {
        }

        public static CounterCreationData ConsumedPerSecond => Cached.Instance.Value.Data[0];
        public static CounterCreationData TotalReceived => Cached.Instance.Value.Data[1];
        public static CounterCreationData ConsumeDuration => Cached.Instance.Value.Data[2];
        public static CounterCreationData ConsumeDurationBase => Cached.Instance.Value.Data[3];
        public static CounterCreationData ConsumeFaulted => Cached.Instance.Value.Data[4];
        public static CounterCreationData ConsumeFaultPercentage => Cached.Instance.Value.Data[5];
        public static CounterCreationData ConsumeFaultPercentageBase => Cached.Instance.Value.Data[6];
        public static CounterCreationData SentPerSecond => Cached.Instance.Value.Data[7];
        public static CounterCreationData TotalSent => Cached.Instance.Value.Data[8];
        public static CounterCreationData SendFaulted => Cached.Instance.Value.Data[9];
        public static CounterCreationData SendFaultPercentage => Cached.Instance.Value.Data[10];
        public static CounterCreationData SendFaultPercentageBase => Cached.Instance.Value.Data[11];
        public static CounterCreationData PublishedPerSecond => Cached.Instance.Value.Data[12];
        public static CounterCreationData TotalPublished => Cached.Instance.Value.Data[13];
        public static CounterCreationData PublishFaulted => Cached.Instance.Value.Data[14];
        public static CounterCreationData PublishFaultPercentage => Cached.Instance.Value.Data[15];
        public static CounterCreationData PublishFaultPercentageBase => Cached.Instance.Value.Data[16];

        protected override IEnumerable<CounterCreationData> GetCounterData()
        {
            yield return Convert(BuiltInCounters.Messages.Counters.ConsumedPerSecond, PerformanceCounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalConsumed, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.AverageConsumeDuration, PerformanceCounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.AverageConusmeDurationBase, PerformanceCounterType.AverageBase);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalConsumeFaults, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.ConsumeFaultPercent, PerformanceCounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.ConsumeFaultPercentBase, PerformanceCounterType.AverageBase);

            yield return Convert(BuiltInCounters.Messages.Counters.SentPerSecond, PerformanceCounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalSent, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalSendFaults, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.SendFaultPercent, PerformanceCounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.SendFaultPercentBase, PerformanceCounterType.AverageBase);

            yield return Convert(BuiltInCounters.Messages.Counters.PublishesPerSecond, PerformanceCounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalPublished, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalPublishFaults, PerformanceCounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.PublishFaultPercent, PerformanceCounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.PublishFaultPercentBase, PerformanceCounterType.AverageBase);
        }

        public static void Install()
        {
            MessagePerformanceCounters value = Cached.Instance.Value;
        }


        static class Cached
        {
            internal static readonly Lazy<MessagePerformanceCounters> Instance = new Lazy<MessagePerformanceCounters>(() => new MessagePerformanceCounters());
        }
    }
}