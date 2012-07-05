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
    using RequestResponse;
    using RequestResponse.Configurators;

    public static class RequestResponseExtensions
    {
        public static bool PublishRequest<TRequest>(this IServiceBus bus,
            TRequest message,
            Action<RequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            IAsyncRequest<TRequest> request = RequestConfiguratorImpl<TRequest>.Create(bus, message, configureCallback);

            bus.Publish(message, context => context.SetRequestContext(request.RequestId, bus));

            return request.Wait();
        }

        public static IAsyncResult BeginPublishRequest<TRequest>(this IServiceBus bus,
            TRequest message,
            AsyncCallback callback,
            object state,
            Action<RequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            IAsyncRequest<TRequest> request = RequestConfiguratorImpl<TRequest>.Create(bus, message, configureCallback);

            bus.Publish(message, context => context.SetRequestContext(request.RequestId, bus));

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

        public static bool SendRequest<TRequest>(this IEndpoint endpoint,
            TRequest message,
            IServiceBus bus,
            Action<RequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            IAsyncRequest<TRequest> request = RequestConfiguratorImpl<TRequest>.Create(bus, message, configureCallback);

            endpoint.Send(message, context => context.SetRequestContext(request.RequestId, bus));

            return request.Wait();
        }

        public static IAsyncResult BeginSendRequest<TRequest>(this IEndpoint endpoint,
            TRequest message,
            IServiceBus bus,
            AsyncCallback callback,
            object state,
            Action<RequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            IAsyncRequest<TRequest> request = RequestConfiguratorImpl<TRequest>.Create(bus, message, configureCallback);

            endpoint.Send(message, context => context.SetRequestContext(request.RequestId, bus));

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

        public static void SetRequestContext<T>(this ISendContext<T> context, string requestId, IServiceBus bus)
            where T : class
        {
            SetRequestContext(context, requestId, bus.Endpoint.Address.Uri);
        }

        public static void SetRequestContext<T>(this ISendContext<T> context, string requestId, Uri responseAddress)
            where T : class
        {
            context.SetRequestId(requestId);
            context.SetSourceAddress(responseAddress);
            context.SendResponseTo(responseAddress);
            context.SendFaultTo(responseAddress);
        }

        public static void SetRequestContext<T>(this IPublishContext<T> context, string requestId, IServiceBus bus)
            where T : class
        {
            SetRequestContext(context, requestId, bus.Endpoint.Address.Uri);
        }

        public static void SetRequestContext<T>(this IPublishContext<T> context, string requestId, Uri responseAddress)
            where T : class
        {
            context.SetRequestId(requestId);
            context.SendResponseTo(responseAddress);
            context.SendFaultTo(responseAddress);
        }
    }
}