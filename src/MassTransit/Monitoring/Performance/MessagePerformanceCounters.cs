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


    public class MessagePerformanceCounters :
        BasePerformanceCounters
    {

#if !NETCORE
        MessagePerformanceCounters()
            : base(BuiltInCounters.Messages.Category.Name, BuiltInCounters.Messages.Category.Help)
        {
        }
#endif

        public static CounterData ConsumedPerSecond => Cached.Instance.Value.Data[0];
        public static CounterData TotalReceived => Cached.Instance.Value.Data[1];
        public static CounterData ConsumeDuration => Cached.Instance.Value.Data[2];
        public static CounterData ConsumeDurationBase => Cached.Instance.Value.Data[3];
        public static CounterData ConsumeFaulted => Cached.Instance.Value.Data[4];
        public static CounterData ConsumeFaultPercentage => Cached.Instance.Value.Data[5];
        public static CounterData ConsumeFaultPercentageBase => Cached.Instance.Value.Data[6];
        public static CounterData SentPerSecond => Cached.Instance.Value.Data[7];
        public static CounterData TotalSent => Cached.Instance.Value.Data[8];
        public static CounterData SendFaulted => Cached.Instance.Value.Data[9];
        public static CounterData SendFaultPercentage => Cached.Instance.Value.Data[10];
        public static CounterData SendFaultPercentageBase => Cached.Instance.Value.Data[11];
        public static CounterData PublishedPerSecond => Cached.Instance.Value.Data[12];
        public static CounterData TotalPublished => Cached.Instance.Value.Data[13];
        public static CounterData PublishFaulted => Cached.Instance.Value.Data[14];
        public static CounterData PublishFaultPercentage => Cached.Instance.Value.Data[15];
        public static CounterData PublishFaultPercentageBase => Cached.Instance.Value.Data[16];

        protected override IEnumerable<CounterData> GetCounterData()
        {
            yield return Convert(BuiltInCounters.Messages.Counters.ConsumedPerSecond, CounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalConsumed, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.AverageConsumeDuration, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.AverageConusmeDurationBase, CounterType.AverageBase);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalConsumeFaults, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.ConsumeFaultPercent, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.ConsumeFaultPercentBase, CounterType.AverageBase);

            yield return Convert(BuiltInCounters.Messages.Counters.SentPerSecond, CounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalSent, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalSendFaults, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.SendFaultPercent, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.SendFaultPercentBase, CounterType.AverageBase);

            yield return Convert(BuiltInCounters.Messages.Counters.PublishesPerSecond, CounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalPublished, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.TotalPublishFaults, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Messages.Counters.PublishFaultPercent, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Messages.Counters.PublishFaultPercentBase, CounterType.AverageBase);
        }

        public static void Install()
        {
            _ = Cached.Instance.Value;
        }


        static class Cached
        {
            internal static readonly Lazy<MessagePerformanceCounters> Instance = new Lazy<MessagePerformanceCounters>(() => new MessagePerformanceCounters());
        }
    }
}
