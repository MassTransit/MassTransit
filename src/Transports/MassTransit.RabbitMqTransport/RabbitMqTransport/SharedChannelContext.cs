namespace MassTransit.RabbitMqTransport;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Middleware;
using RabbitMQ.Client;


public class SharedChannelContext :
    ProxyPipeContext,
    ChannelContext
{
    readonly ChannelContext _context;

    public SharedChannelContext(ChannelContext context, CancellationToken cancellationToken)
        : base(context)
    {
        _context = context;
        CancellationToken = cancellationToken;
    }

    public override CancellationToken CancellationToken { get; }

    public IChannel Channel => _context.Channel;

    public ConnectionContext ConnectionContext => _context.ConnectionContext;

    public async Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, BasicProperties basicProperties, byte[] body, bool awaitAck,
        CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, awaitAck, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments,
        CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.ExchangeBind(destination, source, routingKey, arguments, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments,
        CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.ExchangeDeclare(exchange, type, durable, autoDelete, arguments, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task ExchangeDeclarePassive(string exchange, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.ExchangeDeclarePassive(exchange, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments,
        CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.QueueBind(queue, exchange, routingKey, arguments, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<QueueDeclareOk> QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments,
        CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.QueueDeclare(queue, durable, exclusive, autoDelete, arguments, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<QueueDeclareOk> QueueDeclarePassive(string queue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.QueueDeclarePassive(queue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<uint> QueuePurge(string queue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.QueuePurge(queue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task BasicQos(uint prefetchSize, ushort prefetchCount, bool global, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.BasicQos(prefetchSize, prefetchCount, global, tokenSource.Token).ConfigureAwait(false);
    }

    public async ValueTask BasicAck(ulong deliveryTag, bool multiple, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.BasicAck(deliveryTag, multiple, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task BasicNack(ulong deliveryTag, bool multiple, bool requeue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.BasicNack(deliveryTag, multiple, requeue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<string> BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IAsyncBasicConsumer consumer,
        string consumerTag, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.BasicConsume(queue, noAck, exclusive, arguments, consumer, consumerTag, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task BasicCancel(string consumerTag, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.BasicCancel(consumerTag, tokenSource.Token).ConfigureAwait(false);
    }

    public void NotifyFaulted(Exception exception, Uri contextInputAddress)
    {
        _context.NotifyFaulted(exception, contextInputAddress);
    }
}
