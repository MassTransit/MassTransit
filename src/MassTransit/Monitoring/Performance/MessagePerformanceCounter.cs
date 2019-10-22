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
    using System;
    using Metadata;
    using Util;


    public class MessagePerformanceCounter<TMessage> :
        IDisposable,
        IMessagePerformanceCounter
    {
        readonly IPerformanceCounter _consumedPerSecond;
        readonly IPerformanceCounter _consumeDuration;
        readonly IPerformanceCounter _consumeDurationBase;
        readonly IPerformanceCounter _faulted;
        readonly IPerformanceCounter _faultPercentage;
        readonly IPerformanceCounter _faultPercentageBase;
        readonly IPerformanceCounter _totalConsumed;
        readonly IPerformanceCounter _publishedPerSecond;
        readonly IPerformanceCounter _publishFaulted;
        readonly IPerformanceCounter _publishFaultPercentage;
        readonly IPerformanceCounter _publishFaultPercentageBase;
        readonly IPerformanceCounter _sendFaulted;
        readonly IPerformanceCounter _sendFaultPercentage;
        readonly IPerformanceCounter _sendFaultPercentageBase;
        readonly IPerformanceCounter _sentPerSecond;
        readonly IPerformanceCounter _totalPublished;
        readonly IPerformanceCounter _totalSent;

        public MessagePerformanceCounter(ICounterFactory factory)
        {
            string messageType = TypeMetadataCache<TMessage>.ShortName;
            if (messageType.Length > 127)
                messageType = messageType.Substring(messageType.Length - 127);

            _totalConsumed = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.TotalReceived.CounterName, messageType);
            _consumedPerSecond = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumedPerSecond.CounterName, messageType);
            _consumeDuration = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeDuration.CounterName, messageType);
            _consumeDurationBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeDurationBase.CounterName, messageType);
            _faulted = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeFaulted.CounterName, messageType);
            _faultPercentage = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeFaultPercentage.CounterName, messageType);
            _faultPercentageBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeFaultPercentageBase.CounterName, messageType);
            _totalSent = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.TotalSent.CounterName, messageType);
            _sentPerSecond = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SentPerSecond.CounterName, messageType);
            _sendFaulted = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SendFaulted.CounterName, messageType);
            _sendFaultPercentage = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SendFaultPercentage.CounterName, messageType);
            _sendFaultPercentageBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SendFaultPercentageBase.CounterName, messageType);
            _totalPublished = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.TotalPublished.CounterName, messageType);
            _publishedPerSecond = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishedPerSecond.CounterName, messageType);
            _publishFaulted = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishFaulted.CounterName, messageType);
            _publishFaultPercentage = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishFaultPercentage.CounterName, messageType);
            _publishFaultPercentageBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishFaultPercentageBase.CounterName, messageType);
        }

        public void Dispose()
        {
            _consumeDuration.Dispose();
            _consumeDurationBase.Dispose();
            _consumedPerSecond.Dispose();
            _faultPercentage.Dispose();
            _faultPercentageBase.Dispose();
            _faulted.Dispose();
            _totalConsumed.Dispose();
            _totalSent.Dispose();
            _sentPerSecond.Dispose();
            _sendFaulted.Dispose();
            _sendFaultPercentage.Dispose();
            _sendFaultPercentageBase.Dispose();
            _totalPublished.Dispose();
            _publishedPerSecond.Dispose();
            _publishFaulted.Dispose();
            _publishFaultPercentage.Dispose();
            _publishFaultPercentageBase.Dispose();
        }

        public void Consumed(TimeSpan duration)
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _consumeDuration.IncrementBy((long)duration.TotalMilliseconds);
            _consumeDurationBase.Increment();

            _faultPercentageBase.Increment();
        }

        public void ConsumeFaulted(TimeSpan duration)
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _faulted.Increment();

            _faultPercentage.Increment();
            _faultPercentageBase.Increment();
        }

        public void Sent()
        {
            _totalSent.Increment();
            _sentPerSecond.Increment();

            _sendFaultPercentageBase.Increment();
        }

        public void Published()
        {
            _totalPublished.Increment();
            _publishedPerSecond.Increment();

            _publishFaultPercentageBase.Increment();
        }

        public void PublishFaulted()
        {
            _totalPublished.Increment();
            _publishedPerSecond.Increment();

            _publishFaulted.Increment();

            _publishFaultPercentage.Increment();
            _publishFaultPercentageBase.Increment();
        }

        public void SendFaulted()
        {
            _totalSent.Increment();
            _sentPerSecond.Increment();

            _sendFaulted.Increment();

            _sendFaultPercentage.Increment();
            _sendFaultPercentageBase.Increment();
        }
    }
}
