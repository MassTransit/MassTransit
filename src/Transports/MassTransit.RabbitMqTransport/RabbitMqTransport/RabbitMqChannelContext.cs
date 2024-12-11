namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;


    public class RabbitMqChannelContext :
        ScopePipeContext,
        ChannelContext,
        IAsyncDisposable
    {
        readonly IChannel _channel;
        readonly ConnectionContext _connectionContext;

        public RabbitMqChannelContext(ConnectionContext connectionContext, IChannel channel, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            _connectionContext = connectionContext;
            _channel = channel;
            CancellationToken = cancellationToken;

            _channel.ContinuationTimeout = _connectionContext.ContinuationTimeout;
        }

        public override CancellationToken CancellationToken { get; }

        public IChannel Channel => _channel;

        ConnectionContext ChannelContext.ConnectionContext => _connectionContext;

        public Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, BasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            Task PublishAndMaybeAwaitAck()
            {
                var task = _channel.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, new ReadOnlyMemory<byte>(body), CancellationToken);

                async Task WaitAck()
                {
                    try
                    {
                        await task.ConfigureAwait(false);
                    }
                    catch (PublishException exception) when (exception.IsReturn)
                    {
                        throw new MessageReturnedException("The message was returned by RabbitMQ", exception);
                    }
                }

                return awaitAck ? WaitAck() : Task.CompletedTask;
            }

            return PublishAndMaybeAwaitAck();
        }

        public Task ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return _channel.ExchangeBindAsync(destination, source, routingKey, arguments, false, CancellationToken);
        }

        public Task ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _channel.ExchangeDeclareAsync(exchange, type, durable, autoDelete, arguments, false, CancellationToken);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return _channel.ExchangeDeclarePassiveAsync(exchange, CancellationToken);
        }

        public Task QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return _channel.QueueBindAsync(queue, exchange, routingKey, arguments, false, CancellationToken);
        }

        Task<QueueDeclareOk> ChannelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _channel.QueueDeclareAsync(queue, durable, exclusive, autoDelete, arguments, false, CancellationToken);
        }

        public Task<QueueDeclareOk> QueueDeclarePassive(string queue)
        {
            return _channel.QueueDeclarePassiveAsync(queue, CancellationToken);
        }

        public Task<uint> QueuePurge(string queue)
        {
            return _channel.QueuePurgeAsync(queue, CancellationToken);
        }

        public Task BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return _channel.BasicQosAsync(prefetchSize, prefetchCount, global, CancellationToken);
        }

        public ValueTask BasicAck(ulong deliveryTag, bool multiple)
        {
            if (_channel.IsClosed)
            {
                throw new OperationInterruptedException(
                    new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {_channel.CloseReason}"));
            }

            return _channel.BasicAckAsync(deliveryTag, multiple, CancellationToken);
        }

        public async Task BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            if (_channel.IsClosed)
                return;

            try
            {
                await _channel.BasicNackAsync(deliveryTag, multiple, requeue, CancellationToken).ConfigureAwait(false);
            }
            catch (AlreadyClosedException) // if we are shutting down, the broker would already nack prefetched messages anyway
            {
            }
        }

        public Task<string> BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IAsyncBasicConsumer consumer,
            string consumerTag, CancellationToken cancellationToken)
        {
            return _channel.BasicConsumeAsync(queue, noAck, consumerTag, false, exclusive, arguments, consumer, cancellationToken);
        }

        public async Task BasicCancel(string consumerTag)
        {
            await _channel.BasicCancelAsync(consumerTag, false, CancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            const string message = "ChannelContext Disposed";

            await _channel.Cleanup(200, message, CancellationToken).ConfigureAwait(false);
        }
    }
}
