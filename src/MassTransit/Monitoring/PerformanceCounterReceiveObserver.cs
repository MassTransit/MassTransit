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
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;


    public class PerformanceCounterReceiveObserver :
        IReceiveObserver
    {
        readonly ConcurrentDictionary<string, ConsumerPerformanceCounter> _consumers;
 
        public PerformanceCounterReceiveObserver()
        {
            _consumers = new ConcurrentDictionary<string, ConsumerPerformanceCounter>();
        }

        public async Task PreReceive(ReceiveContext context)
        {
        }

        public async Task PostReceive(ReceiveContext context)
        {
        }

        public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            GetConsumer(consumerType).Consumed(duration);
        }

        ConsumerPerformanceCounter GetConsumer(string consumerType)
        {
            return _consumers.GetOrAdd(consumerType, x => new ConsumerPerformanceCounter(x));
        }

        public async Task ConsumeFault<T>(ConsumeContext<T> context, string consumerType, Exception exception) where T : class
        {
            GetConsumer(consumerType).Faulted();
        }

        public async Task ReceiveFault(ReceiveContext context, Exception exception)
        {
        }
    }


    public class ReceiveEndpointPerformanceCounters
    {
    }
}