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
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An observer that updates the performance counters using the bus events
    /// generated.
    /// </summary>
    public class PerformanceCounterReceiveObserver :
        IReceiveObserver
    {
        Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            ConsumerPerformanceCounterCache.GetCounter(consumerType).Consumed(duration);
            MessagePerformanceCounterCache<T>.Counter.Consumed(duration);
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
        {
            ConsumerPerformanceCounterCache.GetCounter(consumerType).Faulted();
            MessagePerformanceCounterCache<T>.Counter.Faulted();
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }


    public class ReceiveEndpointPerformanceCounters
    {
    }
}