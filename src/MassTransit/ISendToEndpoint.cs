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


    public interface ISendToEndpoint
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext<T>> Send<T>(T message)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback to modify the SendContext</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext<T>> Send<T>(T message, Action<SendContext<T>> callback)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback to modify the SendContext</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext<T>> Send<T>(T message, Func<SendContext<T>, Task<SendContext<T>>> callback)
            where T : class;

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext> Send(object message);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext> Send(object message, Type messageType);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="callback">Allows the context values to be specified</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext> Send(object message, Action<SendContext> callback);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="callback">Allows the context values to be specified</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext> Send(object message, Func<SendContext, Task<SendContext>> callback);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="callback">Allows the context values to be specified</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext> Send(object message, Type messageType, Action<SendContext> callback);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="callback">Allows the context values to be specified</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext> Send(object message, Type messageType, Func<SendContext, Task<SendContext>> callback);

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext<T>> Send<T>(object values)
            where T : class;

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="callback">A callback method to modify the send context for the message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext<T>> Send<T>(object values, Action<SendContext<T>> callback)
            where T : class;

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="callback">A callback method to modify the send context for the message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<SentContext<T>> Send<T>(object values, Func<SendContext<T>, Task<SendContext<T>>> callback)
            where T : class; 
    }
}