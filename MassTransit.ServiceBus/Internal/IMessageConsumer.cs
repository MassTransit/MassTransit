/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

using System;

namespace MassTransit.ServiceBus.Internal
{
    /// <summary>
    /// A non-generic interface for checking and delivering messages to a service bus handler
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Deliver the message to the handler
        /// </summary>
        /// <param name="bus">The service bus where the message arrived</param>
        /// <param name="envelope">The envelope containing the message</param>
        /// <param name="message">The message being delivered</param>
        void Deliver(IServiceBus bus, IEnvelope envelope, IMessage message);

        /// <summary>
        /// Allows the handler to determine if it will handle the message before retrieving it
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>True if the message will be handled, otherwise false.</returns>
        bool IsHandled(IMessage message);

    }

    ///<summary>
    /// Implemented to handle the registration of handlers for a message type
    ///</summary>
    ///<typeparam name="T">The message type to handle</typeparam>
    public interface IMessageConsumer<T> : IMessageConsumer where T : IMessage
    {
        /// <summary>
        /// Adds a subscription to the message type for the specified handler
        /// </summary>
        /// <param name="callback">The function to call to handle the message</param>
        /// <param name="condition">The condition function to determine if a message will be handled</param>
        void Subscribe(Action<IMessageContext<T>> callback, Predicate<T> condition);

        /// <summary>
        /// Removes a subscription to the message type for the specified handler
        /// </summary>
        /// <param name="callback">The function to call to handle the message</param>
        /// <param name="condition">The condition function to determine if a message will be handled</param>
        void Unsubscribe(Action<IMessageContext<T>> callback, Predicate<T> condition);

        /// <summary>
        /// The number of subscribers handling this message type
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Deliver the message to the handler
        /// </summary>
        /// <param name="bus">The service bus where the message arrived</param>
        /// <param name="envelope">The envelope containing the message</param>
        /// <param name="message">The message being delivered</param>
        void Deliver(IServiceBus bus, IEnvelope envelope, T message);

        /// <summary>
        /// Allows the handler to determine if it will handle the message before retrieving it
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>True if the message will be handled, otherwise false.</returns>
        bool IsHandled(T message);
    }
}