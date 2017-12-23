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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using RabbitMQ.Client;
    using Topology;
    using Util;


    public class SharedModelContext :
        ModelContext,
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly ModelContext _context;
        readonly ITaskParticipant _participant;

        public SharedModelContext(ModelContext context, CancellationToken cancellationToken, ITaskScope scope)
        {
            _context = context;
            _cancellationToken = cancellationToken;

            _participant = scope.CreateParticipant($"{TypeMetadataCache<SharedModelContext>.ShortName} - {context.ConnectionContext.HostSettings.ToDebugString()}");
            _participant.SetReady();
        }

        void IDisposable.Dispose()
        {
            _participant.SetComplete();
        }

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        ConnectionContext ModelContext.ConnectionContext => _context.ConnectionContext;

        public IRabbitMqPublishTopology PublishTopology => _context.PublishTopology;

        Task ModelContext.BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            return _context.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, awaitAck);
        }

        Task ModelContext.ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return _context.ExchangeBind(destination, source, routingKey, arguments);
        }

        Task ModelContext.ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _context.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return _context.ExchangeDeclarePassive(exchange);
        }

        Task ModelContext.QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return _context.QueueBind(queue, exchange, routingKey, arguments);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _context.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclarePassive(string queue)
        {
            return _context.QueueDeclarePassive(queue);
        }

        Task<uint> ModelContext.QueuePurge(string queue)
        {
            return _context.QueuePurge(queue);
        }

        Task ModelContext.BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return _context.BasicQos(prefetchSize, prefetchCount, global);
        }

        void ModelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            _context.BasicAck(deliveryTag, multiple);
        }

        void ModelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            _context.BasicNack(deliveryTag, multiple, requeue);
        }

        Task<string> ModelContext.BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
        {
            return _context.BasicConsume(queue, noAck, consumer);
        }

        public Task BasicCancel(string consumerTag)
        {
            return _context.BasicCancel(consumerTag);
        }

        IModel ModelContext.Model => _context.Model;

        CancellationToken PipeContext.CancellationToken => _cancellationToken;
    }
}