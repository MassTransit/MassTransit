namespace MassTransit.RabbitMqTransport.Middleware;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topology;


/// <summary>
/// Configures the broker with the supplied topology once the channel is created, to ensure
/// that the exchanges, queues, and bindings for the channel are properly configured in RabbitMQ.
/// </summary>
public class ConfigureRabbitMqTopologyFilter<TSettings> :
    IFilter<ChannelContext>
    where TSettings : class
{
    readonly BrokerTopology _brokerTopology;
    readonly TSettings _settings;

    public ConfigureRabbitMqTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
    {
        _settings = settings;
        _brokerTopology = brokerTopology;
    }

    public async Task Send(ChannelContext context, IPipe<ChannelContext> next)
    {
        OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await Configure(context, context.CancellationToken);

        try
        {
            await next.Send(context).ConfigureAwait(false);
        }
        catch (Exception)
        {
            oneTimeContext.Evict();

            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("configureTopology");

        _brokerTopology.Probe(scope);
    }

    public async Task<OneTimeContext<ConfigureTopologyContext<TSettings>>> Configure(ChannelContext context, CancellationToken cancellationToken)
    {
        return await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
        {
            context.GetOrAddPayload(() => _settings);
            return ConfigureTopology(context, cancellationToken);
        }).ConfigureAwait(false);
    }

    async Task ConfigureTopology(ChannelContext context, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, queue, cancellationToken))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.Exchanges.Select(exchange => Declare(context, exchange, cancellationToken))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.QueueBindings.Select(binding => Bind(context, binding, cancellationToken))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.ExchangeBindings.Select(binding => Bind(context, binding, cancellationToken))).ConfigureAwait(false);
    }

    static Task Declare(ChannelContext context, Exchange exchange, CancellationToken cancellationToken)
    {
        RabbitMqLogMessages.DeclareExchange(exchange);

        return context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete, exchange.ExchangeArguments,
            cancellationToken);
    }

    static async Task Declare(ChannelContext context, Queue queue, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await context.QueueDeclare(queue.QueueName, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.QueueArguments, cancellationToken)
                .ConfigureAwait(false);

            RabbitMqLogMessages.DeclareQueue(queue, ok.ConsumerCount, ok.MessageCount);
        }
        catch (Exception exception)
        {
            LogContext.Error?.Log(exception, "Declare queue faulted: {Queue}", queue);

            throw;
        }
    }

    static async Task Bind(ChannelContext context, ExchangeToExchangeBinding binding, CancellationToken cancellationToken)
    {
        RabbitMqLogMessages.BindToExchange(binding);

        await context.ExchangeBind(binding.Destination.ExchangeName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments, cancellationToken)
            .ConfigureAwait(false);
    }

    static async Task Bind(ChannelContext context, ExchangeToQueueBinding binding, CancellationToken cancellationToken)
    {
        RabbitMqLogMessages.BindToQueue(binding);

        await context.QueueBind(binding.Destination.QueueName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments, cancellationToken)
            .ConfigureAwait(false);
    }
}
