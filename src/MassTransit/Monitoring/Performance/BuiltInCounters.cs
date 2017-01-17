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
namespace MassTransit.Monitoring.Performance
{
    public static class BuiltInCounters
    {
        public static class Consumers
        {
            public static readonly CounterCategory Category = new CounterCategory("MassTransit Consumers", "Consumers built using MassTransit");

            public static class Counters
            {
                public static readonly Counter MessagesPerSecond = new Counter("Message/s", "Number of messages consumed per second");
                public static readonly Counter TotalMessages = new Counter("Total Messages", "Total number of messages consumed");
                public static readonly Counter AverageDuration = new Counter("Average Duration", "The average time spent consuming a message");
                public static readonly Counter AverageDurationBase = new Counter("Average Duration Base", "The average time spent consuming a message");
                public static readonly Counter TotalFaults = new Counter("Total Faults", "Total number of consumer faults generated");
                public static readonly Counter FaultPercent = new Counter("Fault %", "The percentage of consumers generating faults");
                public static readonly Counter FaultPercentBase = new Counter("Fault % Base", "The percentage of consumers generating faults");
            }
        }


        public static class Messages
        {
            public static readonly CounterCategory Category = new CounterCategory("MassTransit Messages", "Messages handled by MassTransit");

            public static class Counters
            {
                public static readonly Counter ConsumedPerSecond = new Counter("Consumed/s", "Number of messages consumed per second");
                public static readonly Counter TotalConsumed = new Counter("Consumed", "Total number of messages consumed");
                public static readonly Counter AverageConsumeDuration = new Counter("Average Consume Duration", "The average time spent consuming a message");
                public static readonly Counter AverageConusmeDurationBase = new Counter("Average Consume Duration Base", "The average time spent consuming a message");
                public static readonly Counter TotalConsumeFaults = new Counter("Consume Faults", "Total number of consume faults");
                public static readonly Counter ConsumeFaultPercent = new Counter("Consume Fault %", "The percentage of consumes faulted");
                public static readonly Counter ConsumeFaultPercentBase = new Counter("Consume Fault % Base", "The percentage of consumes faulted");
                public static readonly Counter SentPerSecond = new Counter("Sent/s", "Number of messages sent per second");
                public static readonly Counter TotalSent = new Counter("Sent", "Total number of messages sent");
                public static readonly Counter TotalSendFaults = new Counter("Send Faults", "Total number of send faults");
                public static readonly Counter SendFaultPercent = new Counter("Send Fault %", "The percentage of sends faulted");
                public static readonly Counter SendFaultPercentBase = new Counter("Send Fault % Base", "The percentage of sends faulted");
                public static readonly Counter PublishesPerSecond = new Counter("Published/s", "Number of messages Published per second");
                public static readonly Counter TotalPublished = new Counter("Published", "Total number of messages Published");
                public static readonly Counter TotalPublishFaults = new Counter("Publish Faults", "Total number of Publish faults");
                public static readonly Counter PublishFaultPercent = new Counter("Publish Fault %", "The percentage of Publishes faulted");
                public static readonly Counter PublishFaultPercentBase = new Counter("Publish Fault % Base", "The percentage of Publishes faulted");
            }
        }
    }


    public class CounterCategory
    {
        public CounterCategory(string name, string help)
        {
            Name = name;
            Help = help;
        }

        public string Name { get; }
        public string Help { get; }
    }
    public class Counter
    {
        public Counter(string name, string help)
        {
            Name = name;
            Help = help;
        }

        public string Name { get; }
        public string Help { get; }
    }

}