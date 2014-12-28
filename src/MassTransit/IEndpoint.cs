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
    ///         to create the <see cref="ReceiveContext" /> from the bytes on the wire, which hands the message
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
        ///     Send to the endpoint
        /// </summary>
        /// <typeparam name="T">The type of the message to send</typeparam>
        /// <param name="context">
        ///     Send context to generate the in-transport message from. Contains
        ///     out-of-band data such as message ids, correlation ids, headers, and in-band data
        ///     such as the actual data of the message to send.
        /// </param>

        /// <summary>
        ///     Send a message to an endpoint
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to send</param>
        void Send<T>(T message)
            where T : class;
    }
}