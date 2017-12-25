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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Integration;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Specifications;
    using Topology;
    using Util;


    public class RabbitMqModelContext :
        BasePipeContext,
        ModelContext,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<RabbitMqModelContext>();

        readonly ConnectionContext _connectionContext;
        readonly IModel _model;
        readonly ITaskParticipant _participant;
        readonly ConcurrentDictionary<ulong, PendingPublish> _published;
        readonly IRabbitMqHost _host;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        ulong _publishTagMax;

        public RabbitMqModelContext(ConnectionContext connectionContext, IModel model, ITaskScope taskScope, IRabbitMqHost host)
            : this(connectionContext, model, host,
                taskScope.CreateParticipant($"{TypeMetadataCache<RabbitMqModelContext>.ShortName} - {connectionContext.HostSettings.ToDebugString()}"))
        {
        }

        RabbitMqModelContext(ConnectionContext connectionContext, IModel model, IRabbitMqHost host, ITaskParticipant participant)
            : base(new PayloadCacheScope(connectionContext))
        {
            _connectionContext = connectionContext;
            _model = model;
            _host = host;

            _participant = participant;

            _published = new ConcurrentDictionary<ulong, PendingPublish>();
            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            _model.ModelShutdown += OnModelShutdown;
            _model.BasicAcks += OnBasicAcks;
            _model.BasicNacks += OnBasicNacks;
            _model.BasicReturn += OnBasicReturn;

            if (host.Settings.PublisherConfirmation)
            {
                _model.ConfirmSelect();
            }

            _participant.SetReady();
        }

        public void Dispose()
        {
            Close("ModelContext Disposed");
        }

        IModel ModelContext.Model => _model;

        ConnectionContext ModelContext.ConnectionContext => _connectionContext;

        IRabbitMqPublishTopology ModelContext.PublishTopology => _host.Topology.PublishTopology;

        CancellationToken PipeContext.CancellationToken => _participant.StoppedToken;

        async Task ModelContext.BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body,
            bool awaitAck)
        {
            if (_host.Settings.PublisherConfirmation)
            {
                var pendingPublish = await Task.Factory.StartNew(() => PublishAsync(exchange, routingKey, mandatory, basicProperties, body),
                    _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);

                if (awaitAck)
                {
                    await pendingPublish.Task.ConfigureAwait(false);

                    await Task.Yield();
                }
            }
            else
            {
                await Task.Factory.StartNew(() => Publish(exchange, routingKey, mandatory, basicProperties, body),
                    _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);
            }
        }

        Task ModelContext.ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.ExchangeBind(destination, source, routingKey, arguments),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task ModelContext.ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return Task.Factory.StartNew(() => _model.ExchangeDeclarePassive(exchange),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task ModelContext.QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.QueueBind(queue, exchange, routingKey, arguments),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclarePassive(string queue)
        {
            return Task.Factory.StartNew(() => _model.QueueDeclarePassive(queue),_participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task<uint> ModelContext.QueuePurge(string queue)
        {
            return Task.Factory.StartNew(() => _model.QueuePurge(queue),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task ModelContext.BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return Task.Factory.StartNew(() => _model.BasicQos(prefetchSize, prefetchCount, global),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        void ModelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            _model.BasicAck(deliveryTag, multiple);
        }

        void ModelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            _model.BasicNack(deliveryTag, multiple, requeue);
        }

        Task<string> ModelContext.BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
        {
            return Task.Factory.StartNew(() => _model.BasicConsume(queue, noAck, consumer),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task BasicCancel(string consumerTag)
        {
            return Task.Factory.StartNew(() => _model.BasicCancel(consumerTag),
                _participant.StoppedToken, TaskCreationOptions.None, _taskScheduler);
        }

        void Close(string reason)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Closing model: {0} / {1}", _model.ChannelNumber, _connectionContext.HostSettings.ToDebugString());

            try
            {
                if (_host.Settings.PublisherConfirmation && _model.IsOpen && _published.Count > 0)
                {
                    bool timedOut;
                    _model.WaitForConfirms(TimeSpan.FromSeconds(30), out timedOut);
                    if (timedOut)
                        _log.WarnFormat("Timeout waiting for pending confirms: {0}", _connectionContext.HostSettings.ToDebugString());
                    else
                    {
                        _log.DebugFormat("Pending confirms complete: {0}", _connectionContext.HostSettings.ToDebugString());
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Fault waiting for confirms", ex);
            }

            _model.Cleanup(200, reason);

            _participant.SetComplete();
        }

        void OnBasicReturn(object model, BasicReturnEventArgs args)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("BasicReturn: {0}-{1} {2}", args.ReplyCode, args.ReplyText, args.BasicProperties.MessageId);

            object value;
            if (args.BasicProperties.Headers.TryGetValue("publishId", out value))
            {
                var bytes = value as byte[];
                if (bytes == null)
                    return;

                ulong id;
                if (!ulong.TryParse(Encoding.UTF8.GetString(bytes), out id))
                    return;

                PendingPublish published;
                if (_published.TryRemove(id, out published))
                    published.PublishReturned(args.ReplyCode, args.ReplyText);
            }
        }

        PendingPublish PublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            var publishTag = _model.NextPublishSeqNo;
            _publishTagMax = Math.Max(_publishTagMax, publishTag);
            var pendingPublish = new PendingPublish(_connectionContext, exchange, publishTag);
            try
            {
                _published.AddOrUpdate(publishTag, key => pendingPublish, (key, existing) =>
                {
                    existing.PublishNotConfirmed();
                    return pendingPublish;
                });

                basicProperties.Headers["publishId"] = publishTag.ToString("F0");

                _model.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
            }
            catch
            {
                PendingPublish ignored;
                _published.TryRemove(publishTag, out ignored);

                throw;
            }

            return pendingPublish;
        }

        void Publish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            var publishTag = _model.NextPublishSeqNo;
            _publishTagMax = Math.Max(_publishTagMax, publishTag);

            _model.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
        }

        void OnModelShutdown(object model, ShutdownEventArgs reason)
        {
            _model.ModelShutdown -= OnModelShutdown;
            _model.BasicAcks -= OnBasicAcks;
            _model.BasicNacks -= OnBasicNacks;
            _model.BasicReturn -= OnBasicReturn;

            FaultPendingPublishes();

            _participant.SetComplete();
        }

        void FaultPendingPublishes()
        {
            try
            {
                foreach (var key in _published.Keys)
                {
                    PendingPublish pending;
                    if (_published.TryRemove(key, out pending))
                        pending.PublishNotConfirmed();
                }
            }
            catch (Exception)
            {
            }
        }

        void OnBasicNacks(object model, BasicNackEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (var id in ids)
                {
                    PendingPublish value;
                    if (_published.TryRemove(id, out value))
                        value.Nack();
                }
            }
            else
            {
                PendingPublish value;
                if (_published.TryRemove(args.DeliveryTag, out value))
                    value.Nack();
            }
        }

        void OnBasicAcks(object model, BasicAckEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (var id in ids)
                {
                    PendingPublish value;
                    if (_published.TryRemove(id, out value))
                        value.Ack();
                }
            }
            else
            {
                PendingPublish value;
                if (_published.TryRemove(args.DeliveryTag, out value))
                    value.Ack();
            }
        }
    }
}