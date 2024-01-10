#nullable enable
namespace MassTransit.SqlTransport.Middleware;

using System;
using System.Threading.Tasks;
using Configuration;
using Topology;
using Transports;


/// <summary>
/// Configures the broker with the supplied topology once the model is created, to ensure
/// that the exchanges, queues, and bindings for the model are properly configured in SQS.
/// </summary>
public class ConfigureSqlTopologyFilter<TSettings> :
    IFilter<ClientContext>
    where TSettings : class
{
    readonly BrokerTopology _brokerTopology;
    readonly SqlReceiveEndpointContext? _context;
    readonly TSettings _settings;

    public ConfigureSqlTopologyFilter(TSettings settings, BrokerTopology brokerTopology, SqlReceiveEndpointContext? context = null)
    {
        _settings = settings;
        _brokerTopology = brokerTopology;
        _context = context;
    }

    public async Task Send(ClientContext context, IPipe<ClientContext> next)
    {
        OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
        {
            context.GetOrAddPayload(() => _settings);

            return ConfigureTopology(context);
        }).ConfigureAwait(false);

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

    async Task ConfigureTopology(ClientContext context)
    {
        foreach (var queue in _brokerTopology.Queues)
        {
            var queueId = await CreateQueue(context, queue).ConfigureAwait(false);
            if (queue.QueueName == _context?.InputAddress.GetEndpointName() && _settings is SqlReceiveSettings settings)
                settings.QueueId = queueId;
        }

        foreach (var topic in _brokerTopology.Topics)
            await CreateTopic(context, topic).ConfigureAwait(false);

        foreach (var topicSubscription in _brokerTopology.TopicSubscriptions)
            await CreateTopicSubscription(context, topicSubscription).ConfigureAwait(false);

        foreach (var queueSubscription in _brokerTopology.QueueSubscriptions)
            await CreateQueueSubscription(context, queueSubscription).ConfigureAwait(false);
    }

    static Task CreateTopic(ClientContext context, Topic topic)
    {
        SqlLogMessages.CreateTopic(topic);

        return context.CreateTopic(topic);
    }

    static Task CreateQueueSubscription(ClientContext context, TopicToQueueSubscription subscription)
    {
        SqlLogMessages.CreateQueueSubscription(subscription);

        return context.CreateQueueSubscription(subscription);
    }

    static Task CreateTopicSubscription(ClientContext context, TopicToTopicSubscription subscription)
    {
        SqlLogMessages.CreateTopicSubscription(subscription);

        return context.CreateTopicSubscription(subscription);
    }

    static Task<long> CreateQueue(ClientContext context, Queue queue)
    {
        SqlLogMessages.CreateQueue(queue);

        return context.CreateQueue(queue);
    }
}
