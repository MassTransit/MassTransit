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


    public class ConsumerPerformanceCounter :
        IDisposable,
        IConsumerPerformanceCounter
    {
        readonly IPerformanceCounter _consumeDuration;
        readonly IPerformanceCounter _consumeDurationBase;
        readonly IPerformanceCounter _consumedPerSecond;
        readonly IPerformanceCounter _faultPercentage;
        readonly IPerformanceCounter _faultPercentageBase;
        readonly IPerformanceCounter _faulted;
        readonly IPerformanceCounter _totalConsumed;

        public ConsumerPerformanceCounter(string consumerType)
        {
            if (consumerType.Length > 127)
                consumerType = consumerType.Substring(consumerType.Length - 127);

            _totalConsumed = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.TotalReceived.CounterName, consumerType);
            _consumedPerSecond = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.ConsumedPerSecond.CounterName, consumerType);
            _consumeDuration = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.ConsumeDuration.CounterName, consumerType);
            _consumeDurationBase = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.ConsumeDurationBase.CounterName, consumerType);
            _faulted = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.Faulted.CounterName, consumerType);
            _faultPercentage = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.FaultPercentage.CounterName, consumerType);
            _faultPercentageBase = ConsumerPerformanceCounters.CreateCounter(
                ConsumerPerformanceCounters.FaultPercentageBase.CounterName, consumerType);
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
    }
}