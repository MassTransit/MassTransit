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
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
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

            PublishRequest(bus, message, request);

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

            PublishRequest(bus, message, request);

            return request.BeginAsync(callback, state);
        }


        public static bool EndRequest<TRequest>(this IServiceBus bus, IAsyncResult asyncResult) 
            where TRequest : class
        {
            var request = asyncResult as IAsyncRequest<TRequest>;
            if (request == null)
                throw new ArgumentException("The argument is not an IRequest");

            return request.Wait();
        }

#if NET40
        public static ITaskRequest<TRequest> PublishRequestAsync<TRequest>(this IServiceBus bus, TRequest message,
            Action<TaskRequestConfigurator<TRequest>> configureCallback) 
            where TRequest : class
        {
            var requestConfigurator = new TaskRequestConfiguratorImpl<TRequest>(message);

            configureCallback(requestConfigurator);

            var request = requestConfigurator.Create(bus);

            PublishRequest(bus, message, request);

            return request;
        }
#endif


        public static bool SendRequest<TRequest>(this IEndpoint endpoint,
                                                 TRequest message,
                                                 IServiceBus bus,
                                                 Action<RequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            IAsyncRequest<TRequest> request = RequestConfiguratorImpl<TRequest>.Create(bus, message, configureCallback);

            SendRequest(endpoint, bus, message, request);

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

            SendRequest(endpoint, bus, message, request);

            return request.BeginAsync(callback, state);
        }

        public static bool EndRequest<TRequest>(this IEndpoint endpoint, IAsyncResult asyncResult) 
            where TRequest : class
        {
            var request = asyncResult as IAsyncRequest<TRequest>;
            if (request == null)
                throw new ArgumentException("The argument is not an IRequest");

            return request.Wait();
        }

        static void PublishRequest<TRequest>(IServiceBus bus, TRequest message, IRequest<TRequest> request)
            where TRequest : class
        {
            bus.Publish(message, context =>
                {
                    context.SetRequestId(request.RequestId);
                    context.SendResponseTo(bus.Endpoint.Address.Uri);
                });
        }

        static void SendRequest<TRequest>(IEndpoint endpoint, IServiceBus bus, TRequest message,
                                          IRequest<TRequest> request)
            where TRequest : class
        {
            endpoint.Send(message, context =>
                {
                    context.SetRequestId(request.RequestId);
                    context.SetSourceAddress(bus.Endpoint.Address.Uri);
                    context.SendResponseTo(bus.Endpoint.Address.Uri);
                });
        }
    }
}