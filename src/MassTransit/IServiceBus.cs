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
    using Diagnostics.Introspection;
    using Util;
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
        IDisposable,
        DiagnosticsSource
    {
        /// <summary>
        ///   The endpoint from which messages are received
        /// </summary>
        [NotNull]
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
        /// Gets the control bus that can be used 
        /// to add/remove subscripts, move message 
        /// handlers around and tap runtime metrics
        /// from the service bus.
        /// </summary>
        IServiceBus ControlBus { get; }

        /// <summary>
        /// Gets the endpoint cache. This property is used
        /// by <see cref="GetEndpoint"/> method in turn.
        /// </summary>
        IEndpointCache EndpointCache { get; }

        /// <summary>
        /// <para>Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.</para>
        /// 
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name = "T">The type of the message</typeparam>
        /// <param name = "message">The messages to be published</param>
        /// <param name = "contextCallback">A callback that gives the caller
        /// access to the publish context.</param>
        void Publish<T>(T message, Action<IPublishContext<T>> contextCallback)
            where T : class;

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
    }
}