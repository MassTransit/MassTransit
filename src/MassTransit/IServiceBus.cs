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
    using Pipeline;

    /// <summary>
    ///   The action to call to unsubscribe a previously subscribed consumer
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
        IDisposable
    {
        /// <summary>
        ///   The endpoint from which messages are received
        /// </summary>
        IEndpoint Endpoint { get; }

        /// <summary>
        ///   Publishes a message to all subscribed consumers for the message type
        /// </summary>
        /// <typeparam name = "T">The type of the message</typeparam>
        /// <param name = "message">The messages to be published</param>
        /// <param name = "contextCallback"></param>
        void Publish<T>(T message, Action<IPublishContext<T>> contextCallback)
            where T : class;

        /// <summary>
        ///   Returns the service for the requested interface if it was registered with the service bus
        /// </summary>
        /// <typeparam name = "TService"></typeparam>
        /// <returns></returns>
        TService GetService<TService>()
            where TService : IBusService;

        IInboundMessagePipeline InboundPipeline { get; }

        IOutboundMessagePipeline OutboundPipeline { get; }

        IServiceBus ControlBus { get; }

        IEndpointCache EndpointCache { get; }

        IEndpoint GetEndpoint(Uri address);

        /// <summary>
        ///   Not sure this is going to make it, but trying a new approach.
        /// </summary>
        /// <param name = "configure"></param>
        UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure);
    }
}