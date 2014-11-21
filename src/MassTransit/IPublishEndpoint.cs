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
    using Pipeline;

    /// <summary>
    /// A publish endpoint lets the underlying transport determine the actual endpoint to which
    /// the message is sent. For example, an exchange on RabbitMQ and a topic on Azure Service bus.
    /// </summary>
    public interface IPublishEndpoint
    {
        /// <summary>
        /// <para>Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.</para>
        /// 
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name = "TMessage">The type of the message</typeparam>
        /// <param name = "message">The messages to be published</param>
        Task Publish<TMessage>(TMessage message)
            where TMessage : class;

        /// <summary>
        /// <para>Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.</para>
        /// 
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name = "TMessage">The type of the message</typeparam>
        /// <param name = "message">The messages to be published</param>
        /// <param name="publishPipe"></param>
        Task Publish<TMessage>(TMessage message, IPipe<PublishContext<TMessage>> publishPipe)
            where TMessage : class;

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        Task Publish(object message);

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        Task Publish(object message, Type messageType);

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name = "contextCallback">A callback that gives the caller
        /// access to the publish context.</param>
        Task Publish(object message, Action<PublishContext> contextCallback);

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name = "contextCallback">A callback that gives the caller
        /// access to the publish context.</param>
        Task Publish(object message, Type messageType, Action<PublishContext> contextCallback);

        /// <summary>
        /// <see cref="IServiceBus.Publish{T}"/>: this is a "dynamically"
        /// typed overload - give it an interface as its type parameter,
        /// and a loosely typed dictionary of values and the MassTransit
        /// underlying infrastructure will populate an object instance
        /// with the passed values. It actually does this with DynamicProxy
        /// in the background.
        /// </summary>
        /// <typeparam name="TMessage">The type of the interface or
        /// non-sealed class with all-virtual members.</typeparam>
        /// <param name="values">The dictionary of values to place in the
        /// object instance to implement the interface.</param>
        Task Publish<TMessage>(object values)
            where TMessage : class;

        /// <summary>
        /// <see cref="Publish{T}(MassTransit.IServiceBus,object)"/>: this
        /// overload further takes an action; it allows you to set <see cref="IPublishContext"/>
        /// meta-data. Also <see cref="IServiceBus.Publish{T}"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish</typeparam>
        /// <param name="values">The dictionary of values to become hydrated and
        /// published under the type of the interface.</param>
        /// <param name="contextCallback">The context callback.</param>
        Task Publish<TMessage>(object values, Action<PublishContext<TMessage>> contextCallback)
            where TMessage : class;
    }
}