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
#if NET40
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Configures a request and the associated response handler behavior
    /// </summary>
    /// <typeparam name="TRequest">The message type of the request</typeparam>
    public interface TaskRequestConfigurator<TRequest> :
        RequestConfigurator<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// Configures a handler to be called if a response of the specified type
        /// is received. Once received, the request completes by default unless
        /// overridden by calling Continue on the request.
        /// </summary>
        /// <typeparam name="T">The message type of the response</typeparam>
        /// <param name="handler">The handler to call with the response message</param>
        Task<T> Handle<T>(Action<T> handler)
            where T : class;

        /// <summary>
        /// Configures a handler to be called if a response of the specified type
        /// is received. Once received, the request completes by default unless
        /// overridden by calling Continue on the request.
        /// </summary>
        /// <typeparam name="T">The message type of the response</typeparam>
        /// <param name="handler">The handler to call with the response message</param>
        Task<T> Handle<T>(Action<IConsumeContext<T>, T> handler)
            where T : class;

        /// <summary>
        /// Specifies a handler for a fault published by the request handler
        /// </summary>
        /// <param name="faultCallback"></param>
        Task<Fault<TRequest>> HandleFault(Action<Fault<TRequest>> faultCallback);

        /// <summary>
        /// Specifies a handler for a fault published by the request handler
        /// </summary>
        /// <param name="faultCallback"></param>
        Task<Fault<TRequest>> HandleFault(Action<IConsumeContext<Fault<TRequest>>, Fault<TRequest>> faultCallback);
    }
#endif
}