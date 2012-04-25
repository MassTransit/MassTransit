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
namespace MassTransit
{
    using System;
    using Context;
    using Magnum.Caching;
    using Magnum.Reflection;

    public static class SendExtensions
    {
        static readonly Cache<Type, EndpointObjectSender> _typeCache =
            new GenericTypeCache<EndpointObjectSender>(typeof(EndpointObjectSenderImpl<>));

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
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The endpoint where the message should be sent</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        public static void Send(this IEndpoint endpoint, object message)
        {
            if(message == null)
                throw new ArgumentNullException("message");

            _typeCache[message.GetType()].Send(endpoint, message);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        public static void Send<T>(this IEndpoint endpoint, object values)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            endpoint.Send(message, x => { });
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The endpoint where the message should be sent</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        public static void Send(this IEndpoint endpoint, object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if(messageType == null)
                throw new ArgumentNullException("messageType");

            _typeCache[messageType].Send(endpoint, message);
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
            if (message == null)
                throw new ArgumentNullException("message");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            SendContext<T> context = ContextStorage.CreateSendContext(message);

            contextCallback(context);

            endpoint.Send(context);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The endpoint where the message should be sent</param>
        /// <param name="message">The message object</param>
        /// <param name="contextCallback">Allows the context values to be specified</param>
        public static void Send(this IEndpoint endpoint, object message, Action<ISendContext> contextCallback)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            if(contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            _typeCache[message.GetType()].Send(endpoint, message, contextCallback);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The endpoint where the message should be sent</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="contextCallback">Allows the context values to be specified</param>
        public static void Send(this IEndpoint endpoint, object message, Type messageType, Action<ISendContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            _typeCache[messageType].Send(endpoint, message, contextCallback);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="contextCallback">A callback method to modify the send context for the message</param>
        public static void Send<T>(this IEndpoint endpoint, object values, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            endpoint.Send(message, contextCallback);
        }
    }
}