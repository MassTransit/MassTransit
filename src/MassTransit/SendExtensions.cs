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
    using Context;
    using Magnum.Reflection;

    public static class SendExtensions
    {
        /// <summary>
        /// Send a message to an endpoint
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="message">The message to send</param>
        public static void Send<T>(this IEndpoint endpoint, T message)
            where T : class
        {
            SendContext<T> context = ContextStorage.CreateSendContext(message);

            endpoint.Send(context);
        }

        /// <summary>
        /// Send a message to an endpoint
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">A callback method to modify the send context for the message</param>
        public static void Send<T>(this IEndpoint endpoint, T message, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            SendContext<T> context = ContextStorage.CreateSendContext(message);

            contextCallback(context);

            endpoint.Send(context);
        }

        public static void Send<T>(this IEndpoint endpoint, object values)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            endpoint.Send(message, x => { });
        }

        public static void Send<T>(this IEndpoint endpoint, object values, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            endpoint.Send(message, contextCallback);
        }
    }
}