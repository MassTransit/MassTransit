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
namespace MassTransit.Builders
{
    using System.Net.Mime;
    using GreenPipes;
    using Pipeline;


    /// <summary>
    /// Used to build and configure a service bus instance as it is created
    /// </summary>
    public interface IBusBuilder
    {
        SerializerBuilder CreateSerializerBuilder();

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
        /// Create a consume pipe for the endpoint, using the bus builder. The bus builder may add additional filters
        /// from the bus configuration to the endpoint which can be overridden by the endpoint.
        /// </summary>
        /// <returns></returns>
        IConsumePipe CreateConsumePipe();

        /// <summary>
        /// Create a send pipe for the endpoint, using the bus builder. The bus builder may add additional filters
        /// from the bus configuration to the endpoint.
        /// </summary>
        /// <returns></returns>
        ISendPipe CreateSendPipe();

        /// <summary>
        /// Create a publish pipe for the receive endpoint, using the bus builder.
        /// </summary>
        /// <returns></returns>
        IPublishPipe CreatePublishPipe();

        /// <summary>
        /// Connects a bus observer to the bus to observe lifecycle events on the bus
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectBusObserver(IBusObserver observer);
    }
}