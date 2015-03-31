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
    using System.Diagnostics;


    public class ConsumerPerformanceCounter :
        IDisposable
    {
        readonly PerformanceCounter _consumeDuration;
        readonly PerformanceCounter _consumeDurationBase;
        readonly PerformanceCounter _totalConsumed;
        readonly PerformanceCounter _consumedPerSecond;

        public ConsumerPerformanceCounter(string consumerType)
        {
            _totalConsumed = new PerformanceCounter(ConsumerPerformanceCounterCreationData.CategoryName,
                ConsumerPerformanceCounterCreationData.TotalReceived.CounterName, consumerType, false);
            _consumedPerSecond = new PerformanceCounter(ConsumerPerformanceCounterCreationData.CategoryName,
                ConsumerPerformanceCounterCreationData.ConsumedPerSecond.CounterName, consumerType, false);
            _consumeDuration = new PerformanceCounter(ConsumerPerformanceCounterCreationData.CategoryName,
                ConsumerPerformanceCounterCreationData.ConsumeDuration.CounterName, consumerType, false);
            _consumeDurationBase = new PerformanceCounter(ConsumerPerformanceCounterCreationData.CategoryName,
                ConsumerPerformanceCounterCreationData.ConsumeDurationBase.CounterName, consumerType, false);
        }

        public void Dispose()
        {
            _totalConsumed.Close();
            _totalConsumed.Dispose();

            _consumedPerSecond.Close();
            _consumedPerSecond.Dispose();

            _consumeDuration.Close();
            _consumeDuration.Dispose();

            _consumeDurationBase.Close();
            _consumeDurationBase.Dispose();
        }

        public void Consumed(TimeSpan duration)
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _consumeDuration.IncrementBy((long)duration.TotalMilliseconds);
            _consumeDurationBase.Increment();
        }

        public void Faulted()
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _faulted.Increment();
        }
    }
}