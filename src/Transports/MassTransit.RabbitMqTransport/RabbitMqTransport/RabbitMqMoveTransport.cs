#nullable enable
namespace MassTransit.RabbitMqTransport;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Middleware;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;


public class RabbitMqMoveTransport<TSettings>
    where TSettings : class
{
    readonly string _exchange;
    readonly ConfigureRabbitMqTopologyFilter<TSettings> _topologyFilter;

    protected RabbitMqMoveTransport(string exchange, ConfigureRabbitMqTopologyFilter<TSettings> topologyFilter)
    {
        _topologyFilter = topologyFilter;
        _exchange = exchange;
    }

    protected async Task Move(ReceiveContext context, Action<BasicProperties, SendHeaders> preSend)
    {
        if (!context.TryGetPayload(out ChannelContext? channelContext))
            throw new ArgumentException("The ReceiveContext must contain a ChannelContext", nameof(context));

        if (channelContext.Channel.IsClosed)
        {
            throw new OperationInterruptedException(
                new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {channelContext.Channel.CloseReason}"));
        }

        OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await _topologyFilter.Configure(channelContext).ConfigureAwait(false);

        BasicProperties properties;
        var routingKey = "";
        byte[] body;

        if (context.TryGetPayload(out RabbitMqBasicConsumeContext? basicConsumeContext))
        {
            properties = new BasicProperties(basicConsumeContext.Properties);
            routingKey = basicConsumeContext.RoutingKey;
            body = context.GetBody();
        }
        else
        {
            properties = new BasicProperties { Headers = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase) };
            body = context.GetBody();
        }

        SendHeaders headers = new MoveTransportHeaders(properties);

        headers.SetHostHeaders();

        preSend(properties, headers);

        try
        {
            await channelContext.BasicPublishAsync(_exchange, routingKey, true, properties, body, true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            oneTimeContext?.Evict();
            throw;
        }
    }
}
