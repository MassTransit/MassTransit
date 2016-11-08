// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Net.Mime;
    using Builders;
    using Pipeline;


    public interface IReceiveEndpointBuilder :
        IConsumePipeConnector,
        IConsumeMessageObserverConnector
    {
        IConsumePipe ConsumePipe { get; }

        ISendTransportProvider SendTransportProvider { get; }

        IMessageDeserializer MessageDeserializer { get; }

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(SerializerFactory serializerFactory);

        /// <summary>
        /// Adds an inbound message deserializer to the available deserializers
        /// </summary>
        /// <param name="contentType">The content type of the deserializer</param>
        /// <param name="deserializerFactory">The factory to create the deserializer</param>
        void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory);

        /// <summary>
        /// Creates a send endpoint provider using the bus and supplied specifications
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <param name="specifications"></param>
        /// <returns></returns>
        ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications);

        /// <summary>
        /// Creates a publish endpoint provider using the bus and supplied specifications
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <param name="specifications"></param>
        /// <returns></returns>
        IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications);
    }
}