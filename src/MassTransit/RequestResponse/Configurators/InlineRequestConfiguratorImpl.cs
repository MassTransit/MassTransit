// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse.Configurators
{
    using System;
    using Advanced;

    public class InlineRequestConfiguratorImpl<TRequest> :
        RequestConfiguratorBase<TRequest>,
        InlineRequestConfigurator<TRequest>
        where TRequest : class
    {
        readonly RequestImpl<TRequest> _request;

        public InlineRequestConfiguratorImpl(TRequest message)
            : base(message)
        {
            _request = new RequestImpl<TRequest>(RequestId, Request);
        }

        public void Handle<T>(Action<T> handler)
            where T : class
        {
            AddHandler(typeof(T),
                () => new CompleteResponseHandler<T>(RequestId, _request, RequestSynchronizationContext, handler));
        }

        public void Handle<T>(Action<IConsumeContext<T>, T> handler)
            where T : class
        {
            AddHandler(typeof(T),
                () => new CompleteResponseHandler<T>(RequestId, _request, RequestSynchronizationContext, handler));
        }

        public void Watch<T>(Action<T> watcher)
            where T : class
        {
            AddHandler(typeof(T), () => new WatchResponseHandler<T>(RequestId, RequestSynchronizationContext, watcher));
        }

        public void Watch<T>(Action<IConsumeContext<T>, T> watcher)
            where T : class
        {
            AddHandler(typeof(T), () => new WatchResponseHandler<T>(RequestId, RequestSynchronizationContext, watcher));
        }

        public void HandleFault(Action<Fault<TRequest>> faultCallback)
        {
            AddHandler(typeof(Fault<TRequest>), () => new CompleteResponseHandler<Fault<TRequest>>(RequestId, 
                _request, RequestSynchronizationContext, faultCallback));
        }

        public void HandleFault(Action<IConsumeContext<Fault<TRequest>>, Fault<TRequest>> faultCallback)
        {
            AddHandler(typeof(Fault<TRequest>), () => new CompleteResponseHandler<Fault<TRequest>>(RequestId,
                _request, RequestSynchronizationContext, faultCallback));
        }

        public IAsyncRequest<TRequest> Build(IServiceBus bus)
        {
            _request.SetTimeout(Timeout);
            if (TimeoutHandler != null)
                _request.SetTimeoutHandler(TimeoutHandler);

            UnsubscribeAction unsubscribeAction = bus.Configure(x => Handlers.CombineSubscriptions(h => h.Connect(x)));

            _request.SetUnsubscribeAction(unsubscribeAction);

            return _request;
        }
    }
}