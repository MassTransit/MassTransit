namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Integration;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    public class RabbitMqModelContext :
        ScopePipeContext,
        ModelContext,
        IAsyncDisposable
    {
        readonly ConnectionContext _connectionContext;
        readonly IModel _model;
        readonly CancellationToken _cancellationToken;
        readonly ChannelExecutor _executor;
        readonly PendingConfirmationCollection _confirmations;
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
                _confirmations = new PendingConfirmationCollection(_connectionContext);
            }

            _executor = new ChannelExecutor(1);

            _publisher = connectionContext.BatchSettings.Enabled
                ? (IPublisher)new BatchPublisher(_executor, model, connectionContext.BatchSettings, _confirmations)
                : new ImmediatePublisher(_executor, model, _confirmations);

            _model.ModelShutdown += OnModelShutdown;
            _model.BasicAcks += OnAcknowledged;
            _model.BasicNacks += OnNotAcknowledged;
            _model.BasicReturn += OnBasicReturn;
        }

        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Closing model: {ChannelNumber} {Host}", _model.ChannelNumber, _connectionContext.Description);

            try
            {
                if (_confirmations != null && _model.IsOpen)
                {
                    bool timedOut;
                    do
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        _model.WaitForConfirms(_connectionContext.StopTimeout, out timedOut);
                        if (timedOut)
                            LogContext.Warning?.Log("Timeout waiting for pending confirms:  {ChannelNumber} {Host}", _model.ChannelNumber,
                                _connectionContext.Description);
                    }
                    while (timedOut);
                }
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Fault waiting for pending confirms:  {ChannelNumber} {Host}", _model.ChannelNumber,
                    _connectionContext.Description);
            }

            await _publisher.DisposeAsync(cancellationToken).ConfigureAwait(false);
            await _executor.DisposeAsync(cancellationToken).ConfigureAwait(false);

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

        void ModelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            _model.BasicAck(deliveryTag, multiple);
        }

        void ModelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            _model.BasicNack(deliveryTag, multiple, requeue);
        }

        Task<string> ModelContext.BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            return _executor.Run(() => _model.BasicConsume(consumer, queue, noAck, "", false, exclusive, arguments), CancellationToken);
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
