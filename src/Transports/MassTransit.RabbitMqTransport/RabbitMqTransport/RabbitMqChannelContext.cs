namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Middleware;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using Transports;


    public class RabbitMqChannelContext :
        ScopePipeContext,
        ChannelContext,
        IAsyncDisposable
    {
        readonly IAgent _agent;
        readonly CancellationToken _cancellationToken;
        readonly IChannel _channel;
        CancellationTokenSource _tokenSource;

        public RabbitMqChannelContext(ConnectionContext connectionContext, IChannel channel, IAgent agent, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            ConnectionContext = connectionContext;

            _cancellationToken = cancellationToken;
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(connectionContext.CancellationToken, cancellationToken);

            _channel = channel;
            _agent = agent;

            _channel.ContinuationTimeout = ConnectionContext.ContinuationTimeout;
        }

        public override CancellationToken CancellationToken => _tokenSource?.Token ?? _cancellationToken;

        public IChannel Channel => _channel;

        public ConnectionContext ConnectionContext { get; }

        public Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, BasicProperties basicProperties, byte[] body, bool awaitAck,
            CancellationToken cancellationToken)
        {
            Task PublishAndMaybeAwaitAck()
            {
                var task = _channel.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, new ReadOnlyMemory<byte>(body), cancellationToken);

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
                    catch (PublishException exception)
                    {
                        throw new RabbitMqConnectionException("BasicPublishAsync failed", exception);
                    }
                }

                return awaitAck ? WaitAck() : Task.CompletedTask;
            }

            return PublishAndMaybeAwaitAck();
        }

        public Task ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments,
            CancellationToken cancellationToken)
        {
            return _channel.ExchangeBindAsync(destination, source, routingKey, arguments, false, cancellationToken);
        }

        public Task ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments,
            CancellationToken cancellationToken)
        {
            return _channel.ExchangeDeclareAsync(exchange, type, durable, autoDelete, arguments, false, cancellationToken);
        }

        public Task ExchangeDeclarePassive(string exchange, CancellationToken cancellationToken)
        {
            return _channel.ExchangeDeclarePassiveAsync(exchange, cancellationToken);
        }

        public Task QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            return _channel.QueueBindAsync(queue, exchange, routingKey, arguments, false, cancellationToken);
        }

        public Task<QueueDeclareOk> QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments,
            CancellationToken cancellationToken)
        {
            return _channel.QueueDeclareAsync(queue, durable, exclusive, autoDelete, arguments, false, cancellationToken);
        }

        public Task<QueueDeclareOk> QueueDeclarePassive(string queue, CancellationToken cancellationToken)
        {
            return _channel.QueueDeclarePassiveAsync(queue, cancellationToken);
        }

        public Task<uint> QueuePurge(string queue, CancellationToken cancellationToken)
        {
            return _channel.QueuePurgeAsync(queue, cancellationToken);
        }

        public Task BasicQos(uint prefetchSize, ushort prefetchCount, bool global, CancellationToken cancellationToken)
        {
            return _channel.BasicQosAsync(prefetchSize, prefetchCount, global, cancellationToken);
        }

        public ValueTask BasicAck(ulong deliveryTag, bool multiple, CancellationToken cancellationToken)
        {
            if (_channel.IsClosed)
            {
                throw new OperationInterruptedException(
                    new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {_channel.CloseReason}"));
            }

            return _channel.BasicAckAsync(deliveryTag, multiple, cancellationToken);
        }

        public async Task BasicNack(ulong deliveryTag, bool multiple, bool requeue, CancellationToken cancellationToken)
        {
            if (_channel.IsClosed)
                return;

            try
            {
                await _channel.BasicNackAsync(deliveryTag, multiple, requeue, cancellationToken).ConfigureAwait(false);
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

        public async Task BasicCancel(string consumerTag, CancellationToken cancellationToken)
        {
            await _channel.BasicCancelAsync(consumerTag, false, cancellationToken);
        }

        public void NotifyFaulted(Exception exception, Uri inputAddress)
        {
            Task.Run(() => _agent.Stop($"Unrecoverable exception on {inputAddress.GetEndpointName()}", CancellationToken.None), CancellationToken.None)
                .IgnoreUnobservedExceptions();
        }

        public async ValueTask DisposeAsync()
        {
            const string message = "ChannelContext Disposed";

            await _channel.Cleanup(200, message, CancellationToken).ConfigureAwait(false);

            var tokenSource = _tokenSource;
            _tokenSource = null;
            tokenSource.Dispose();
        }
    }
}
