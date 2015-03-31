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
        AsyncObservable<IReceiveObserver>,
        INotifyReceiveObserver,
        IReceiveObserver
    {
        public void NotifyPreReceive(ReceiveContext context)
        {
            Notify(x => x.PreReceive(context));
        }

        public void NotifyPostReceive(ReceiveContext context)
        {
            Notify(x => x.PostReceive(context));
        }

        public void NotifyPostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            Notify(x => x.PostConsume(context, duration, consumerType));
        }

        public void NotifyConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            Notify(x => x.ConsumeFault(context, duration, consumerType, exception));
        }

        public void NotifyReceiveFault(ReceiveContext context, Exception exception)
        {
            Notify(x => x.ReceiveFault(context, exception));
        }

        async Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            NotifyPreReceive(context);
        }

        async Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            NotifyPostReceive(context);
        }

        async Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            NotifyPostConsume(context, duration, consumerType);
        }

        async Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            NotifyConsumeFault(context, duration, consumerType, exception);
        }

        async Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            NotifyReceiveFault(context, exception);
        }
    }
}