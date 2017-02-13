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
    using Events;
    using MassTransit;


    /// <summary>
    /// A request is a state-machine based request configuration that includes
    /// the events and states related to the execution of a request.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public interface Request<in TInstance, TRequest, TResponse>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// The name of the request
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The settings that are used for the request, including the timeout
        /// </summary>
        RequestSettings Settings { get; }

        /// <summary>
        /// The event that is raised when the request completes and the response is received
        /// </summary>
        Event<TResponse> Completed { get; set; }

        /// <summary>
        /// The event raised when the request faults
        /// </summary>
        Event<Fault<TRequest>> Faulted { get; set; }

        /// <summary>
        /// The event raised when the request times out with no response received
        /// </summary>
        Event<RequestTimeoutExpired<TRequest>> TimeoutExpired { get; set; }

        /// <summary>
        /// The state that is transitioned to once the request is pending
        /// </summary>
        State Pending { get; set; }

        /// <summary>
        /// Sets the requestId on the instance using the configured property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="requestId"></param>
        void SetRequestId(TInstance instance, Guid? requestId);

        /// <summary>
        /// Gets the requestId on the instance using the configured property
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Guid? GetRequestId(TInstance instance);
    }
}