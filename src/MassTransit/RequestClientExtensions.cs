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
namespace MassTransit
{
    using System;


    public static class RequestClientExtensions
    {
        /// <summary>
        /// Creates a request client that uses the bus to retrieve the endpoint and send the request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="bus">The bus on which the client is created</param>
        /// <param name="address">The service address that handles the request</param>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <returns></returns>
        public static IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>(this IBus bus, Uri address, TimeSpan timeout)
            where TRequest : class
            where TResponse : class
        {
            return new MessageRequestClient<TRequest, TResponse>(bus, address, timeout);
        }

        /// <summary>
        /// Creates a request client that uses the bus to publish a request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="bus">The bus on which the client is created</param>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="ttl">The time that the request will live for</param>
        /// <returns></returns>
        public static IRequestClient<TRequest, TResponse> CreatePublishRequestClient<TRequest, TResponse>(this IBus bus, TimeSpan timeout, TimeSpan? ttl)
            where TRequest : class
            where TResponse : class
        {
            return new PublishRequestClient<TRequest, TResponse>(bus, timeout, ttl);
        }
    }
}