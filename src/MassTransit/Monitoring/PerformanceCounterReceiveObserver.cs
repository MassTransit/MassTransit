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
    using System.Threading.Tasks;


    /// <summary>
    /// An observer that updates the performance counters using the bus events
    /// generated.
    /// </summary>
    public class PerformanceCounterReceiveObserver :
        IReceiveObserver
    {
        async Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
        }

        async Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
        }

        async Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            ConsumerPerformanceCounterCache.GetCounter(consumerType).Consumed(duration);
            MessagePerformanceCounterCache<T>.Counter.Consumed(duration);
        }

        async Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, string consumerType, Exception exception)
        {
            ConsumerPerformanceCounterCache.GetCounter(consumerType).Faulted();
            MessagePerformanceCounterCache<T>.Counter.Faulted();
        }

        async Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
        }
    }


    public class ReceiveEndpointPerformanceCounters
    {
    }
}