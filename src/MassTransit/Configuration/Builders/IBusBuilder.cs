// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Builders
{
    using System;
    using System.Net.Mime;
    using GreenPipes;
    using Pipeline;


    /// <summary>
    /// Used to build and configure a service bus instance as it is created
    /// </summary>
    public interface IBusBuilder
    {
        /// <summary>
        /// The Send Transport Provider
        /// </summary>
        ISendTransportProvider SendTransportProvider { get; }

        /// <summary>
        /// The default message deserializer
        /// </summary>
        IMessageDeserializer GetMessageDeserializer(ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider);

        /// <summary>
        /// Creates a send endpoint provider using the bus and supplied specifications
        /// </summary>
        /// <param name="specifications"></param>
        /// <returns></returns>
        ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications);

        /// <summary>
        /// Creates a publish endpoint provider using the bus and supplied specifications
        /// </summary>
        /// <param name="specifications"></param>
        /// <returns></returns>
        IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications);

        /// <summary>
        /// Adds a receive endpoint to the bus
        /// </summary>
        /// <param name="queueName">The name of the queue for the receive endpoint</param>
        /// <param name="receiveEndpoint"></param>
        void AddReceiveEndpoint(string queueName, IReceiveEndpoint receiveEndpoint);

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(Func<IMessageSerializer> serializerFactory);

        /// <summary>
        /// Adds an inbound message deserializer to the available deserializers
        /// </summary>
        /// <param name="contentType">The content type of the deserializer</param>
        /// <param name="deserializerFactory">The factory to create the deserializer</param>
        void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory);

        /// <summary>
        /// Create a consume pipe for the endpoint, using the bus builder. The bus builder may add additional filters
        /// from the bus configuration to the endpoint which can be overridden by the endpoint.
        /// </summary>
        /// <param name="specifications"></param>
        /// <returns></returns>
        IConsumePipe CreateConsumePipe(params IConsumePipeSpecification[] specifications);

        /// <summary>
        /// Create a send pipe for the endpoint, using the bus builder. The bus builder may add additional filters
        /// from the bus configuration to the endpoint.
        /// </summary>
        /// <param name="specifications"></param>
        /// <returns></returns>
        ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications);

        /// <summary>
        /// Connects a bus observer to the bus to observe lifecycle events on the bus
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectBusObserver(IBusObserver observer);
    }
}