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


    public interface RequestConfigurator
    {
        /// <summary>
        /// Returns the identifier assigned to this request
        /// </summary>
        string RequestId { get; }

        /// <summary>
        /// Specifies a time-to-live (TTL) for the request message after which the message
        /// should be discarded.
        /// </summary>
        /// <param name="expiration">The time-to-live for the message</param>
        void SetRequestExpiration(TimeSpan expiration);

        /// <summary>
        /// Specifies a timeout period after which the request should be cancelled and
        /// a TimeoutException should be thrown
        /// </summary>
        /// <param name="timeout">The timeout period</param>
        void SetTimeout(TimeSpan timeout);

        /// <summary>
        /// Specifies a timeout period after which the request should be cancelled and
        /// the timeoutCallback invoked.
        /// </summary>
        /// <param name="timeout">The time period</param>
        /// <param name="timeoutCallback">The callback to invoke</param>
        void HandleTimeout(TimeSpan timeout, Action timeoutCallback);
    }

    /// <summary>
    /// Configures a request and the associated response handler behavior
    /// </summary>
    /// <typeparam name="TRequest">The message type of the request</typeparam>
    public interface RequestConfigurator<TRequest> :
        RequestConfigurator
        where TRequest : class
    {
        /// <summary>
        /// The request message that was sent
        /// </summary>
        TRequest Request { get; }

        /// <summary>
        /// Specifies a timeout period after which the request should be cancelled
        /// </summary>
        /// <param name="timeout">The timeout period</param>
        /// <param name="timeoutCallback"></param>
        void HandleTimeout(TimeSpan timeout, Action<TRequest> timeoutCallback);

        /// <summary>
        /// Configures a watcher to be called when a specified type is received. Messages
        /// received do not complete the request, but are merely observed while the request
        /// is pending.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="watcher"></param>
        void Watch<T>(Action<T> watcher)
            where T : class;

        /// <summary>
        /// Configures a watcher to be called when a specified type is received. Messages
        /// received do not complete the request, but are merely observed while the request
        /// is pending.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="watcher"></param>
        void Watch<T>(Action<IConsumeContext<T>, T> watcher)
            where T : class;
    }
}