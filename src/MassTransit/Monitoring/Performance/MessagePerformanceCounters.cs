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
        PerformanceCounters
    {
        public const string CategoryName = "MassTransit Messages";
        public const string CategoryHelp = "Messages handled by MassTransit";

        MessagePerformanceCounters()
            : base(CategoryName, CategoryHelp)
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

        public static IPerformanceCounter CreateCounter(string counterName, string instanceName)
        {
            return Cached.Instance.Value.CreatePerformanceCounter(counterName, instanceName);
        }

        protected override IEnumerable<CounterCreationData> GetCounterData()
        {
            yield return
                new CounterCreationData("Consumed/s", "Number of messages consumed per second", PerformanceCounterType.RateOfCountsPerSecond32);
            yield return
                new CounterCreationData("Consumed", "Total number of messages consumed", PerformanceCounterType.NumberOfItems64);
            yield return
                new CounterCreationData("Average Consume Duration", "The average time spent consuming a message", PerformanceCounterType.AverageCount64);
            yield return
                new CounterCreationData("Average Consume Duration Base", "The average time spent consuming a message", PerformanceCounterType.AverageBase);
            yield return
                new CounterCreationData("Consume Faults", "Total number of consume faults", PerformanceCounterType.NumberOfItems64);
            yield return
                new CounterCreationData("Consume Fault %", "The percentage of consumes faulted", PerformanceCounterType.AverageCount64);
            yield return
                new CounterCreationData("Consume Fault % Base", "The percentage of consumes faulted", PerformanceCounterType.AverageBase);
            yield return
                new CounterCreationData("Sent/s", "Number of messages sent per second", PerformanceCounterType.RateOfCountsPerSecond32);
            yield return
                new CounterCreationData("Sent", "Total number of messages sent", PerformanceCounterType.NumberOfItems64);
            yield return
                new CounterCreationData("Send Faults", "Total number of send faults", PerformanceCounterType.NumberOfItems64);
            yield return
                new CounterCreationData("Send Fault %", "The percentage of sends faulted", PerformanceCounterType.AverageCount64);
            yield return
                new CounterCreationData("Send Fault % Base", "The percentage of sends faulted", PerformanceCounterType.AverageBase);
            yield return
                new CounterCreationData("Published/s", "Number of messages Published per second", PerformanceCounterType.RateOfCountsPerSecond32);
            yield return
                new CounterCreationData("Published", "Total number of messages Published", PerformanceCounterType.NumberOfItems64);
            yield return
                new CounterCreationData("Publish Faults", "Total number of Publish faults", PerformanceCounterType.NumberOfItems64);
            yield return
                new CounterCreationData("Publish Fault %", "The percentage of Publishes faulted", PerformanceCounterType.AverageCount64);
            yield return
                new CounterCreationData("Publish Fault % Base", "The percentage of Publishes faulted", PerformanceCounterType.AverageBase);
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