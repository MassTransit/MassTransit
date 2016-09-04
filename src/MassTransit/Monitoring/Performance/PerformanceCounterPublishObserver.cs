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
    public class PerformanceCounterPublishObserver<TFactory> :
        IPublishObserver
        where TFactory : ICounterFactory, new()
    {
        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            MessagePerformanceCounterCache<TFactory, T>.Counter.Published();

            return TaskUtil.Completed;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            MessagePerformanceCounterCache<TFactory, T>.Counter.PublishFaulted();

            return TaskUtil.Completed;
        }
    }
}