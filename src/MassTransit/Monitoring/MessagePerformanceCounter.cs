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
    using Util;


    public class MessagePerformanceCounter<TMessage> :
        IDisposable,
        IMessagePerformanceCounter
    {
        readonly IPerformanceCounter _consumeDuration;
        readonly IPerformanceCounter _consumeDurationBase;
        readonly IPerformanceCounter _consumedPerSecond;
        readonly IPerformanceCounter _faultPercentage;
        readonly IPerformanceCounter _faultPercentageBase;
        readonly IPerformanceCounter _faulted;
        readonly IPerformanceCounter _totalConsumed;

        public MessagePerformanceCounter()
        {
            string messageType = TypeMetadataCache<TMessage>.ShortName;
            if (messageType.Length > 127)
                messageType = messageType.Substring(messageType.Length - 127);

            _totalConsumed = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.TotalReceived.CounterName, messageType);
            _consumedPerSecond = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.ConsumedPerSecond.CounterName, messageType);
            _consumeDuration = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.ConsumeDuration.CounterName, messageType);
            _consumeDurationBase = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.ConsumeDurationBase.CounterName, messageType);
            _faulted = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.Faulted.CounterName, messageType);
            _faultPercentage = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.FaultPercentage.CounterName, messageType);
            _faultPercentageBase = MessagePerformanceCounters.CreateCounter(
                MessagePerformanceCounters.FaultPercentageBase.CounterName, messageType);
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
        }

        public void Consumed(TimeSpan duration)
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _consumeDuration.IncrementBy((long)duration.TotalMilliseconds);
            _consumeDurationBase.Increment();

            _faultPercentageBase.Increment();
        }

        public void Faulted()
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _faulted.Increment();

            _faultPercentage.Increment();
            _faultPercentageBase.Increment();
        }
    }
}