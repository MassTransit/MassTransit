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
namespace MassTransit.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class ReceiveObservable :
        Connectable<IReceiveObserver>,
        INotifyReceiveObserver,
        IReceiveObserver
    {
        public Task NotifyPreReceive(ReceiveContext context)
        {
            return ForEachAsync(x => x.PreReceive(context));
        }

        public Task NotifyPostReceive(ReceiveContext context)
        {
            return ForEachAsync(x => x.PostReceive(context));
        }

        public Task NotifyPostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return ForEachAsync(x => x.PostConsume(context, duration, consumerType));
        }

        public Task NotifyConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return ForEachAsync(x => x.ConsumeFault(context, duration, consumerType, exception));
        }

        public Task NotifyReceiveFault(ReceiveContext context, Exception exception)
        {
            return ForEachAsync(x => x.ReceiveFault(context, exception));
        }

        Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            return NotifyPreReceive(context);
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            return NotifyPostReceive(context);
        }

        Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return NotifyPostConsume(context, duration, consumerType);
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyConsumeFault(context, duration, consumerType, exception);
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            return NotifyReceiveFault(context, exception);
        }
    }
}