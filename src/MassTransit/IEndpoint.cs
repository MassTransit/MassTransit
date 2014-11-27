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
    using Serialization;
    using Transports;


    /// <summary>
    ///     <para>IEndpoint is implemented by an endpoint. An endpoint is an addressable location on the network.</para>
    ///     <para>
    ///         In MassTransit, the endpoint ties together the inbound transport, the outbound transport,
    ///         the error transport that ships problematic messages to the error queue, mesage retry trackers
    ///         and serialization.
    ///     </para>
    ///     <para>
    ///         It is up to the transports themselves to implement the correct connection handling and to
    ///         to create the <see cref="IReceiveContext" /> from the bytes on the wire, which hands the message
    ///         over to MassTransit's internals.
    ///     </para>
    /// </summary>
    public interface IEndpoint :
        IDisposable
    {
        /// <summary>
        ///     The address of the endpoint
        /// </summary>
        EndpointAddress Address { get; }

        /// <summary>
        ///     The inbound transport for the endpoint
        /// </summary>
        IInboundTransport InboundTransport { get; }

        /// <summary>
        ///     The outbound transport for the endpoint
        /// </summary>
        IOutboundTransport OutboundTransport { get; }

        /// <summary>
        ///     The transport where faulting messages (poison messages) are sent
        /// </summary>
        IOutboundTransport ErrorTransport { get; }

        /// <summary>
        ///     The message serializer being used by the endpoint
        /// </summary>
        IMessageSerializer Serializer { get; }

        /// <summary>
        ///     Send to the endpoint
        /// </summary>
        /// <typeparam name="T">The type of the message to send</typeparam>
        /// <param name="context">
        ///     Send context to generate the in-transport message from. Contains
        ///     out-of-band data such as message ids, correlation ids, headers, and in-band data
        ///     such as the actual data of the message to send.
        /// </param>
        void Send<T>(ISendContext<T> context)
            where T : class;

        /// <summary>
        ///     Send a message to an endpoint
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to send</param>
        void Send<T>(T message)
            where T : class;

        /// <summary>
        ///     Send a message to an endpoint
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">A callback method to modify the send context for the message</param>
        void Send<T>(T message, Action<ISendContext<T>> contextCallback)
            where T : class;

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        void Send(object message);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        void Send(object message, Type messageType);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="contextCallback">Allows the context values to be specified</param>
        void Send(object message, Action<ISendContext> contextCallback);

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="contextCallback">Allows the context values to be specified</param>
        void Send(object message, Type messageType, Action<ISendContext> contextCallback);

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="values">The property values to initialize on the interface</param>
        void Send<T>(object values)
            where T : class;

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="contextCallback">A callback method to modify the send context for the message</param>
        void Send<T>(object values, Action<ISendContext<T>> contextCallback)
            where T : class;

        /// <summary>
        ///     <para>
        ///         Receive from the endpoint by passing a function that can preview the message: if the caller
        ///         chooses to accept it, return a method that will consume the message. The first argument of the
        ///         Func is created by the transport and this is what callers of receive must inspect
        ///         to find what receivers (the return value Action{IReceiveContext}) are interested in the
        ///         received data.
        ///     </para>
        ///     <para>Returns after the specified timeout if no message is available.</para>
        /// </summary>
        /// <param name="receiver">The function to preview/consume the message</param>
        /// <param name="timeout">The time to wait for a message to be available</param>
        void Receive(Func<IReceiveContext, Action<IReceiveContext>> receiver, TimeSpan timeout);
    }
}