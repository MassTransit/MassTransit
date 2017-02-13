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
namespace Automatonymous
{
    using System;
    using Activities;
    using Binders;
    using Events;
    using MassTransit.Context;


    public static class RequestExtensions
    {
        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(
            this EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request,
            EventMessageFactory<TInstance, TData, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, messageFactory);

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider">A provider for the address used for the request</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(
            this EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressProvider<TInstance, TData> serviceAddressProvider, EventMessageFactory<TInstance, TData, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, serviceAddressProvider, messageFactory);

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request, messageFactory);

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressProvider<TInstance, TData, TException> serviceAddressProvider,
            EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request, serviceAddressProvider, messageFactory);

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(
            this EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request,
            EventMessageFactory<TInstance, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TRequest, TResponse>(request, messageFactory);

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(
            this EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressProvider<TInstance> serviceAddressProvider, EventMessageFactory<TInstance, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TRequest, TResponse>(request, serviceAddressProvider, messageFactory);

            return binder.Add(activity);
        }

        /// <summary>
        /// Cancels the request timeout, and clears the request data from the state instance
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="binder"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> CancelRequestTimeout<TInstance, TData, TRequest, TResponse>(
            this EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            var activity = new CancelRequestTimeoutActivity<TInstance, TData, TRequest, TResponse>(request);

            return binder.Add(activity);
        }

        /// <summary>
        /// Clears the requestId on the state
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="binder"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> ClearRequest<TInstance, TData, TRequest, TResponse>(
            this EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            var activity = new ClearRequestActivity<TInstance, TData, TRequest, TResponse>(request);

            return binder.Add(activity);
        }
    }
}