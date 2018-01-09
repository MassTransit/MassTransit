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
namespace MassTransit.RabbitMqTransport
{
    using RabbitMQ.Client;


    /// <summary>
    ///     Contains the context of the BasicConsume call received by the BasicConsumer
    ///     bound to the inbound RabbitMQ model
    /// </summary>
    public interface RabbitMqBasicConsumeContext
    {
        /// <summary>
        ///     The exchange to which to the message was sent
        /// </summary>
        string Exchange { get; }

        /// <summary>
        ///     The routing key specified
        /// </summary>
        string RoutingKey { get; }

        /// <summary>
        ///     The consumer tag of the receiving consumer
        /// </summary>
        string ConsumerTag { get; }

        /// <summary>
        ///     The delivery tag of the message to the consumer
        /// </summary>
        ulong DeliveryTag { get; }

        /// <summary>
        ///     The basic properties of the message
        /// </summary>
        IBasicProperties Properties { get; }
        
        /// <summary>
        /// The message body, since it's a byte array on RabbitMQ
        /// </summary>
        byte[] Body { get; }
    }
}