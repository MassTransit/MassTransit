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
namespace MassTransit.RabbitMqTransport
{
    using RabbitMQ.Client;


    public interface RabbitMqSendContext :
        SendContext
    {
        /// <summary>
        ///     Specify that the published message must be delivered to a queue or it will be returned
        /// </summary>
        bool Mandatory { get; set; }

        /// <summary>
        /// The destination exchange for the message
        /// </summary>
        string Exchange { get; }

        /// <summary>
        /// The routing key for the message (defaults to "")
        /// </summary>
        string RoutingKey { get; set; }

        /// <summary>
        /// True if the ack from the broker should be awaited, otherwise only the BasicPublish call is awaited
        /// </summary>
        bool AwaitAck { get; set; }

        /// <summary>
        /// The basic properties for the RabbitMQ message
        /// </summary>
        IBasicProperties BasicProperties { get; }
    }


    public interface RabbitMqSendContext<out T> :
        SendContext<T>,
        RabbitMqSendContext
        where T : class
    {
    }
}