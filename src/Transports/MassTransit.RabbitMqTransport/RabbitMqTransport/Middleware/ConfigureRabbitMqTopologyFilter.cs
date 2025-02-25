namespace MassTransit.RabbitMqTransport.Middleware;

using System;
using System.Linq;
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
        OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await Configure(context);

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

    public async Task<OneTimeContext<ConfigureTopologyContext<TSettings>>> Configure(ChannelContext context)
    {
        return await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
        {
            context.GetOrAddPayload(() => _settings);
            return ConfigureTopology(context);
        }).ConfigureAwait(false);
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("configureTopology");

        _brokerTopology.Probe(scope);
    }

    async Task ConfigureTopology(ChannelContext context)
    {
        await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, queue))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.Exchanges.Select(exchange => Declare(context, exchange))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.QueueBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.ExchangeBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);
    }

    static Task Declare(ChannelContext context, Exchange exchange)
    {
        RabbitMqLogMessages.DeclareExchange(exchange);

        return context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete, exchange.ExchangeArguments);
    }

    static async Task Declare(ChannelContext context, Queue queue)
    {
        try
        {
            var ok = await context.QueueDeclare(queue.QueueName, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.QueueArguments)
                .ConfigureAwait(false);

            RabbitMqLogMessages.DeclareQueue(queue, ok.ConsumerCount, ok.MessageCount);

            await Task.Delay(10).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            LogContext.Error?.Log(exception, "Declare queue faulted: {Queue}", queue);

            throw;
        }
    }

    static async Task Bind(ChannelContext context, ExchangeToExchangeBinding binding)
    {
        RabbitMqLogMessages.BindToExchange(binding);

        await context.ExchangeBind(binding.Destination.ExchangeName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments)
            .ConfigureAwait(false);

        await Task.Delay(10).ConfigureAwait(false);
    }

    static async Task Bind(ChannelContext context, ExchangeToQueueBinding binding)
    {
        RabbitMqLogMessages.BindToQueue(binding);

        await context.QueueBind(binding.Destination.QueueName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments).ConfigureAwait(false);

        await Task.Delay(10).ConfigureAwait(false);
    }
}
