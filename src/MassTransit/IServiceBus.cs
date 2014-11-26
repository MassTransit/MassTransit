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
    using Diagnostics.Introspection;
    using Pipeline;


    /// <summary>
    ///   The action to call to unsubscribe a previously subscribed consumer.
    /// </summary>
    /// <returns></returns>
    public delegate bool UnsubscribeAction();


    /// <summary>
    ///   The action to call to unregister a previously registered component
    /// </summary>
    /// <returns></returns>
    public delegate bool UnregisterAction();


    /// <summary>
    ///   The base service bus interface
    /// </summary>
    public interface IServiceBus :
        IBus,
        IDisposable,
        DiagnosticsSource
    {
        /// <summary>
        ///   The endpoint from which messages are received
        /// </summary>
        IEndpoint Endpoint { get; }

        /// <summary>
        /// Gets the inbound message pipeline.
        /// </summary>
        IInboundMessagePipeline InboundPipeline { get; }

        /// <summary>
        /// Gets the outbound message pipeline.
        /// </summary>
        IOutboundMessagePipeline OutboundPipeline { get; }

        /// <summary>
        /// Gets the endpoint cache. This property is used
        /// by <see cref="GetEndpoint"/> method in turn.
        /// </summary>
        IEndpointCache EndpointCache { get; }

        /// <summary>
        /// Gets or Sets the timeout used to wait for consumers to finish when shutting the bus down.
        /// </summary>
        TimeSpan ShutdownTimeout { get; set; }

        /// <summary>
        /// Looks an endpoint up by its uri.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>The endpoint that corresponds to the uri passed</returns>
        IEndpoint GetEndpoint(Uri address);

        /// <summary>
        ///   Not sure this is going to make it, but trying a new approach.
        /// </summary>
        /// <param name = "configure"></param>
        /// <returns>An unsubscribe action that can be called to unsubscribe
        /// what was configured to be subscribed with the func passed. <see cref="UnsubscribeAction"/>.</returns>
        UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure);

        /// <summary>
        /// Get the first service with the matching type, throwing an InvalidOperationException if none is found.
        /// </summary>
        /// <param name="type">The type of service to get.</param>
        /// <returns>The first service of type T.</returns>
        IBusService GetService(Type type);

        /// <summary>
        /// Try to get the first service with the matching type.
        /// </summary>
        /// <param name="type">The type of service to get.</param>
        /// <param name="result">The service.</param>
        /// <returns>Whether the service was found.</returns>
        bool TryGetService(Type type, out IBusService result);
    }
}