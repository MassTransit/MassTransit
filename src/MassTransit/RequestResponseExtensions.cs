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
namespace MassTransit
{
    using System;
    using RequestResponse.Configurators;

    public static class RequestResponseExtensions
    {
        public static bool PublishRequest<TRequest>(this IServiceBus bus, TRequest message,
            Action<InlineRequestConfigurator<TRequest>> configureCallback,
            Action<IPublishContext<TRequest>> contextCallback = null)
            where TRequest : class
        {
            var configurator = new InlineRequestConfiguratorImpl<TRequest>(message);
            configureCallback(configurator);

            IAsyncRequest<TRequest> request = configurator.Build(bus);

            bus.Publish(message, context =>
            {
                configurator.ApplyContext(context, bus.Endpoint.Address.Uri);
                if (contextCallback != null)
                    contextCallback(context);
            });

            return request.Wait();
        }

        public static IAsyncResult BeginPublishRequest<TRequest>(this IServiceBus bus, TRequest message,
            AsyncCallback callback, object state,
            Action<InlineRequestConfigurator<TRequest>> configureCallback,
            Action<IPublishContext<TRequest>> contextCallback = null)
            where TRequest : class
        {
            var configurator = new InlineRequestConfiguratorImpl<TRequest>(message);
            configureCallback(configurator);

            IAsyncRequest<TRequest> request = configurator.Build(bus);

            bus.Publish(message, context =>
            {
                configurator.ApplyContext(context, bus.Endpoint.Address.Uri);
                if (contextCallback != null)
                    contextCallback(context);
            });

            return request.BeginAsync(callback, state);
        }

        public static bool EndPublishRequest<TRequest>(this IServiceBus bus, IAsyncResult asyncResult)
            where TRequest : class
        {
            var request = asyncResult as IAsyncRequest<TRequest>;
            if (request == null)
                throw new ArgumentException("The argument is not an IRequest");

            return request.Wait();
        }

        public static bool SendRequest<TRequest>(this IEndpoint endpoint, TRequest message, IServiceBus bus,
            Action<InlineRequestConfigurator<TRequest>> configureCallback,
            Action<ISendContext<TRequest>> contextCallback = null)
            where TRequest : class
        {
            var configurator = new InlineRequestConfiguratorImpl<TRequest>(message);
            configureCallback(configurator);

            IAsyncRequest<TRequest> request = configurator.Build(bus);

            endpoint.Send(message, context =>
            {
                configurator.ApplyContext(context, bus.Endpoint.Address.Uri);
                if (contextCallback != null)
                    contextCallback(context);
            });

            return request.Wait();
        }

        public static IAsyncResult BeginSendRequest<TRequest>(this IEndpoint endpoint, TRequest message, IServiceBus bus,
            AsyncCallback callback, object state, Action<InlineRequestConfigurator<TRequest>> configureCallback,
            Action<ISendContext<TRequest>> contextCallback = null)
            where TRequest : class
        {
            var configurator = new InlineRequestConfiguratorImpl<TRequest>(message);

            configureCallback(configurator);
            IAsyncRequest<TRequest> request = configurator.Build(bus);

            endpoint.Send(message, context =>
            {
                configurator.ApplyContext(context, bus.Endpoint.Address.Uri);
                if (contextCallback != null)
                    contextCallback(context);
            });

            return request.BeginAsync(callback, state);
        }

        public static bool EndSendRequest<TRequest>(this IEndpoint endpoint, IAsyncResult asyncResult)
            where TRequest : class
        {
            var request = asyncResult as IAsyncRequest<TRequest>;
            if (request == null)
                throw new ArgumentException("The argument is not an IRequest");

            return request.Wait();
        }
    }
}