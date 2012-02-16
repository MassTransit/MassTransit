// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Pipeline;
    using SubscriptionConnectors;

    public class RequestConfiguratorImpl<TRequest> :
        RequestConfigurator<TRequest>
        where TRequest : class
    {
        readonly IList<Func<IInboundPipelineConfigurator, UnsubscribeAction>> _handlers;
        readonly TRequest _message;
        readonly RequestImpl<TRequest> _request;
        readonly string _requestId;

        public RequestConfiguratorImpl(TRequest message)
        {
            _message = message;
            _handlers = new List<Func<IInboundPipelineConfigurator, UnsubscribeAction>>();
            _requestId = NewId.NextGuid().ToString();

            _request = new RequestImpl<TRequest>(_requestId, message);
        }

        public TRequest Request
        {
            get { return _message; }
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        public void Handle<TResponse>(Action<TResponse> handler)
            where TResponse : class
        {
            var connector = new RequestHandlerSubscriptionConnector<TResponse>();

            Action<TResponse> responseHandler = message =>
                {
                    try
                    {
                        handler(message);

                        _request.Complete(message);
                    }
                    catch (Exception ex)
                    {
                        var exception = new RequestException("The response handler threw an exception", ex, message);
                        _request.Fail(exception);
                    }
                };

            _handlers.Add(
                x => { return connector.Connect(x, _requestId, HandlerSelector.ForHandler(responseHandler)); });
        }

        public void HandleTimeout(TimeSpan timeout, Action timeoutCallback)
        {
            _request.SetTimeout(timeout);
            _request.SetTimeoutCallback(timeoutCallback);
        }

        public void SetTimeout(TimeSpan timeout)
        {
            _request.SetTimeout(timeout);
        }

        IRequest<TRequest> Build(IServiceBus bus)
        {
            UnsubscribeAction unsubscribeAction = bus.Configure(configurator =>
                {
                    UnsubscribeAction seed = () => true;

                    return _handlers.Aggregate(seed, (x, handlerConfigurator) => x + handlerConfigurator(configurator));
                });

            _request.SetUnsubscribeAction(unsubscribeAction);

            return _request;
        }

        public static IRequest<TRequest> Create(IServiceBus bus, TRequest message,
                                                Action<RequestConfigurator<TRequest>> configureCallback)
        {
            var configurator = new RequestConfiguratorImpl<TRequest>(message);

            configureCallback(configurator);

            return configurator.Build(bus);
        }
    }
}