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
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An observer that updates the performance counters using the bus events
    /// generated.
    /// </summary>
    public class PerformanceCounterReceiveObserver :
        IReceiveObserver
    {
        readonly ICounterFactory _factory;

        public PerformanceCounterReceiveObserver(ICounterFactory factory)
        {
            _factory = factory;
        }

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
            ConsumerPerformanceCounterCache.GetCounter(_factory, consumerType).Consumed(duration);
            MessagePerformanceCounterCache<T>.Counter(_factory).Consumed(duration);
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            ConsumerPerformanceCounterCache.GetCounter(_factory, consumerType).Faulted();
            MessagePerformanceCounterCache<T>.Counter(_factory).ConsumeFaulted(duration);
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}