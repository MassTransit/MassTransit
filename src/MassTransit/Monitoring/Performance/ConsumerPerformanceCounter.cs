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


    /// <summary>
    /// Tracks the consumption and failure of a consumer processing messages. The message types
    /// in this case are not included in the counter, only the consumer itself.
    /// </summary>
    public class ConsumerPerformanceCounter :
        IDisposable,
        IConsumerPerformanceCounter
    {
        readonly IPerformanceCounter _duration;
        readonly IPerformanceCounter _durationBase;
        readonly IPerformanceCounter _consumeRate;
        readonly IPerformanceCounter _faultPercentage;
        readonly IPerformanceCounter _faultPercentageBase;
        readonly IPerformanceCounter _totalFaults;
        readonly IPerformanceCounter _totalMessages;

        public ConsumerPerformanceCounter(string consumerType)
        {
            if (consumerType.Length > 127)
                consumerType = consumerType.Substring(consumerType.Length - 127);

            _totalMessages = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.TotalMessages.CounterName, consumerType);
            _consumeRate = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.ConsumeRate.CounterName, consumerType);
            _duration = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.Duration.CounterName, consumerType);
            _durationBase = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.DurationBase.CounterName, consumerType);
            _totalFaults = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.TotalFaults.CounterName, consumerType);
            _faultPercentage = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.FaultPercentage.CounterName, consumerType);
            _faultPercentageBase = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.FaultPercentageBase.CounterName, consumerType);
        }

        public void Consumed(TimeSpan duration)
        {
            _totalMessages.Increment();
            _consumeRate.Increment();

            _duration.IncrementBy((long)duration.TotalMilliseconds);
            _durationBase.Increment();

            _faultPercentageBase.Increment();
        }

        public void Faulted()
        {
            _totalMessages.Increment();
            _consumeRate.Increment();

            _totalFaults.Increment();

            _faultPercentage.Increment();
            _faultPercentageBase.Increment();
        }

        public void Dispose()
        {
            _duration.Dispose();
            _durationBase.Dispose();
            _consumeRate.Dispose();
            _faultPercentage.Dispose();
            _faultPercentageBase.Dispose();
            _totalFaults.Dispose();
            _totalMessages.Dispose();
        }
    }
}