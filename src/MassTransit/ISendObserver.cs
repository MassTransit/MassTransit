// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    /// Observes messages as they are sent to transports. These should not be used to intercept or
    /// filter messages, in that case a filter should be created and registered on the transport.
    /// </summary>
    public interface ISendObserver
    {
        /// <summary>
        /// Called before the message is sent to the transport
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns></returns>
        Task PreSend<T>(SendContext<T> context)
            where T : class;

        /// <summary>
        /// Called after the message is sent to the transport (and confirmed by the transport if supported)
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns></returns>
        Task PostSend<T>(SendContext<T> context)
            where T : class;

        /// <summary>
        /// Called when the message fails to send to the transport, including the exception that was thrown
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <param name="exception">The exception from the transport</param>
        /// <returns></returns>
        Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class;
    }
}