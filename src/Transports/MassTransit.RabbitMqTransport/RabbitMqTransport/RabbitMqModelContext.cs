namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Util;


    public class RabbitMqModelContext :
        ScopePipeContext,
        ModelContext,
        IAsyncDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly PendingConfirmationCollection _confirmations;
        readonly ConnectionContext _connectionContext;
        readonly ChannelExecutor _executor;
        readonly IModel _model;
        readonly IPublisher _publisher;

        public RabbitMqModelContext(ConnectionContext connectionContext, IModel model, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            _connectionContext = connectionContext;
            _model = model;
            _cancellationToken = cancellationToken;

            if (_connectionContext.PublisherConfirmation)
            {
                _model.ConfirmSelect();
                _confirmations = new PendingConfirmationCollection();
            }

            _model.ContinuationTimeout = _connectionContext.ContinuationTimeout;

            _executor = new ChannelExecutor(1);

            _publisher = connectionContext.BatchSettings.Enabled
                ? (IPublisher)new BatchPublisher(_executor, model, connectionContext.BatchSettings, _confirmations)
                : new ImmediatePublisher(_executor, model, _confirmations);

            _model.ModelShutdown += OnModelShutdown;
            _model.BasicAcks += OnAcknowledged;
            _model.BasicNacks += OnNotAcknowledged;
            _model.BasicReturn += OnBasicReturn;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_confirmations != null && _model.IsOpen)
                {
                    _model.WaitForConfirms(_connectionContext.StopTimeout, out var timedOut);
                    if (timedOut)
                    {
                        LogContext.Warning?.Log("Timeout waiting for pending confirms:  {ChannelNumber} {Host}", _model.ChannelNumber,
                            _connectionContext.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Fault waiting for pending confirms:  {ChannelNumber} {Host}", _model.ChannelNumber,
                    _connectionContext.Description);
            }

            await _publisher.DisposeAsync().ConfigureAwait(false);
            await _executor.DisposeAsync().ConfigureAwait(false);

            const string message = "ModelContext Disposed";

            _model.Cleanup(200, message);
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        IModel ModelContext.Model => _model;

        ConnectionContext ModelContext.ConnectionContext => _connectionContext;

        Task ModelContext.BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            return _publisher.Publish(exchange, routingKey, mandatory, basicProperties, body, awaitAck);
        }

        Task ModelContext.ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return _executor.Run(() => _model.ExchangeBind(destination, source, routingKey, arguments), CancellationToken);
        }

        Task ModelContext.ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _executor.Run(() => _model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments), CancellationToken);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return _executor.Run(() => _model.ExchangeDeclarePassive(exchange), CancellationToken);
        }

        Task ModelContext.QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return _executor.Run(() => _model.QueueBind(queue, exchange, routingKey, arguments), CancellationToken);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _executor.Run(() => _model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments), CancellationToken);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclarePassive(string queue)
        {
            return _executor.Run(() => _model.QueueDeclarePassive(queue), CancellationToken);
        }

        Task<uint> ModelContext.QueuePurge(string queue)
        {
            return _executor.Run(() => _model.QueuePurge(queue), CancellationToken);
        }

        Task ModelContext.BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return _executor.Run(() => _model.BasicQos(prefetchSize, prefetchCount, global), CancellationToken);
        }

        Task ModelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            return _model.IsClosed
                ? TaskUtil.Faulted<bool>(new InvalidOperationException($"The channel was closed: {_model.CloseReason} {_model.ChannelNumber}"))
                : _executor.Run(() => _model.BasicAck(deliveryTag, multiple), CancellationToken);
        }

        async Task ModelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            if (_model.IsClosed)
                return;

            try
            {
                await _executor.Run(() => _model.BasicNack(deliveryTag, multiple, requeue), CancellationToken).ConfigureAwait(false);
            }
            catch (ChannelClosedException) // if we are shutting down, the broker would already nack prefetched messages anyway
            {
            }
        }

        Task<string> ModelContext.BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer,
            string consumerTag)
        {
            return _executor.Run(() => _model.BasicConsume(consumer, queue, noAck, consumerTag, false, exclusive, arguments), CancellationToken);
        }

        public Task BasicCancel(string consumerTag)
        {
            return _executor.Run(() => _model.BasicCancel(consumerTag), CancellationToken);
        }

        void OnBasicReturn(object model, BasicReturnEventArgs args)
        {
            LogContext.Debug?.Log("BasicReturn: {ReplyCode}-{ReplyText} {MessageId}", args.ReplyCode, args.ReplyText, args.BasicProperties.MessageId);

            if (_confirmations == null)
                return;

            if (!args.BasicProperties.Headers.TryGetValue("publishId", out var value))
                return;

            var deliveryTag = value switch
            {
                byte[] bytes when ulong.TryParse(Encoding.UTF8.GetString(bytes), out var id) => id,
                string s when ulong.TryParse(s, out var id) => id,
                _ => default
            };

            if (deliveryTag != 0)
                _confirmations.Returned(deliveryTag, args.ReplyCode, args.ReplyText);
        }

        void OnModelShutdown(object model, ShutdownEventArgs reason)
        {
            _model.ModelShutdown -= OnModelShutdown;
            _model.BasicAcks -= OnAcknowledged;
            _model.BasicNacks -= OnNotAcknowledged;
            _model.BasicReturn -= OnBasicReturn;

            _confirmations?.NotConfirmed(reason.ReplyText);
        }

        void OnNotAcknowledged(object model, BasicNackEventArgs args)
        {
            _confirmations?.NotAcknowledged(args.DeliveryTag, args.Multiple);
        }

        void OnAcknowledged(object model, BasicAckEventArgs args)
        {
            _confirmations?.Acknowledged(args.DeliveryTag, args.Multiple);
        }
    }
}
