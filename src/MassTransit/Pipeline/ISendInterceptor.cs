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
namespace MassTransit.Pipeline
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Allows a message to be intercepted during Send
    /// this is probably a bad idea given the use of middleware as a better method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISendInterceptor<in T>
        where T : class, PipeContext
    {
        /// <summary>
        /// Called before the message is serialized
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PreSerialize(SendContext<T> context);

        /// <summary>
        /// Called before the message is sent to the transport
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PreSend(SendContext<T> context);

        /// <summary>
        /// Called after the message has been sent to the transport
        /// </summary>
        /// <param name="context">The send context</param>
        /// <returns></returns>
        Task PostSend(SendContext<T> context);

        /// <summary>
        /// Called after the message has been acknowledged by the transport.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Acknowledged(SendContext<T> context);

        /// <summary>
        /// Called if the message has failed to send or was not (or negatively) acknowledged by the transport.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task SendFaulted(SendContext<T> context, Exception ex);
    }
}