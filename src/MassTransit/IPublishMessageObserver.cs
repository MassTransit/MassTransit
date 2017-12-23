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
    using System.Threading.Tasks;


    /// <summary>
    /// Observes messages as they are published via a publish endpoint. These should not be used to intercept or
    /// filter messages, in that case a filter should be created and registered on the transport.
    /// </summary>
    public interface IPublishMessageObserver<in T>
        where T : class
    {
        /// <summary>
        /// Called before the message is sent to the transport
        /// </summary>
        /// <param name="context">The message send context</param>
        /// <returns></returns>
        Task PrePublish(PublishContext<T> context);

        /// <summary>
        /// Called after the message is sent to the transport (and confirmed by the transport if supported)
        /// </summary>
        /// <param name="context">The message send context</param>
        /// <returns></returns>
        Task PostPublish(PublishContext<T> context);

        /// <summary>
        /// Called when the message fails to send to the transport, including the exception that was thrown
        /// </summary>
        /// <param name="context">The message send context</param>
        /// <param name="exception">The exception from the transport</param>
        /// <returns></returns>
        Task PublishFault(PublishContext<T> context, Exception exception);
    }
}