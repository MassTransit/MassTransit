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

    /// <summary>
    /// Configures a request and the associated response handler behavior
    /// </summary>
    /// <typeparam name="TRequest">The message type of the request</typeparam>
    /// <typeparam name="TKey">The correlation key for the request and response messages</typeparam>
    public interface RequestConfigurator<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// The request message that was sent
        /// </summary>
        TRequest Request { get; }

        /// <summary>
        /// Returns the identifier assigned to this request
        /// </summary>
        string RequestId { get; }

        /// <summary>
        /// Configures a handler to be called if a response of the specified type
        /// is received. Once received, the request completes by default unless
        /// overridden by calling Continue on the request.
        /// </summary>
        /// <typeparam name="TResponse">The message type of the response</typeparam>
        /// <param name="handler">The handler to call with the response message</param>
        void Handle<TResponse>(Action<TResponse> handler)
            where TResponse : class;

        /// <summary>
        /// Specifies a timeout period after which the request should be cancelled
        /// </summary>
        /// <param name="timeout">The timeout period</param>
        /// <param name="timeoutCallback"></param>
        void HandleTimeout(TimeSpan timeout, Action timeoutCallback);

        /// <summary>
        /// Specifies a timeout period after which the request should be cancelled and
        /// a TimeoutException should be thrown
        /// </summary>
        /// <param name="timeout">The timeout period</param>
        void SetTimeout(TimeSpan timeout);
    }
}