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
namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using RabbitMQ.Client;


    /// <summary>
    /// With a connect, and a model from RabbitMQ, this context is passed forward to allow
    /// the model to be configured and connected
    /// </summary>
    public interface ModelContext :
        PipeContext
    {
        /// <summary>
        /// The model
        /// </summary>
        IModel Model { get; }

        /// <summary>
        /// The connection context on which the model was created
        /// </summary>
        ConnectionContext ConnectionContext { get; }

        /// <summary>
        /// Publish a message to the broker, asynchronously
        /// </summary>
        /// <param name="exchange">The destination exchange</param>
        /// <param name="routingKey">The exchange routing key</param>
        /// <param name="mandatory">true if the message must be delivered</param>
        /// <param name="basicProperties">The message properties</param>
        /// <param name="body">The message body</param>
        /// <param name="awaitAck"></param>
        /// <returns>An awaitable Task that is completed when the message is acknowledged by the broker</returns>
        Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck);

        Task ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments);
        Task ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments);
        Task ExchangeDeclarePassive(string exchange);

        Task QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments);
        Task<QueueDeclareOk> QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments);
        Task<QueueDeclareOk> QueueDeclarePassive(string queue);

        Task<uint> QueuePurge(string queue);

        Task BasicQos(uint prefetchSize, ushort prefetchCount, bool global);

        void BasicAck(ulong deliveryTag, bool multiple);
        void BasicNack(ulong deliveryTag, bool multiple, bool requeue);

        Task<string> BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer);

        Task BasicCancel(string consumerTag);
    }
}