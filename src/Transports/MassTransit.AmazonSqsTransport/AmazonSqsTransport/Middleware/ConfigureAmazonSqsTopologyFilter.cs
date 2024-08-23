namespace MassTransit.AmazonSqsTransport.Middleware;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topology;


/// <summary>
/// Configures the broker with the supplied topology once the model is created, to ensure
/// that the exchanges, queues, and bindings for the model are properly configured in AmazonSQS.
/// </summary>
public class ConfigureAmazonSqsTopologyFilter<TSettings> :
    IFilter<ClientContext>
    where TSettings : class
{
    readonly BrokerTopology _brokerTopology;
    readonly SqsReceiveEndpointContext _context;
    readonly TSettings _settings;

    public ConfigureAmazonSqsTopologyFilter(TSettings settings, BrokerTopology brokerTopology, SqsReceiveEndpointContext context = null)
    {
        _settings = settings;
        _brokerTopology = brokerTopology;
        _context = context;
    }

    public async Task Send(ClientContext context, IPipe<ClientContext> next)
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

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("configureTopology");

        _brokerTopology.Probe(scope);
    }

    public async Task<OneTimeContext<ConfigureTopologyContext<TSettings>>> Configure(ClientContext context)
    {
        return await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
        {
            context.GetOrAddPayload(() => _settings);

            if (_context != null && AnyAutoDelete())
                _context.AddSendAgent(new RemoveAmazonSqsTopologyAgent(context, _brokerTopology));

            return ConfigureTopology(context);
        }).ConfigureAwait(false);
    }

    async Task ConfigureTopology(ClientContext context)
    {
        IEnumerable<Task<TopicInfo>> topics = _brokerTopology.Topics.Select(topic => Declare(context, topic));

        IEnumerable<Task<QueueInfo>> queues = _brokerTopology.Queues.Select(queue => Declare(context, queue));

        await Task.WhenAll(topics).ConfigureAwait(false);
        await Task.WhenAll(queues).ConfigureAwait(false);

        IEnumerable<Task> subscriptions = _brokerTopology.QueueSubscriptions.Select(subscription => Declare(context, subscription));
        await Task.WhenAll(subscriptions).ConfigureAwait(false);
    }

    bool AnyAutoDelete()
    {
        return _brokerTopology.Topics.Any(x => x.AutoDelete) || _brokerTopology.Queues.Any(x => x.AutoDelete);
    }

    static async Task<TopicInfo> Declare(ClientContext context, Topic topic)
    {
        var topicInfo = await context.CreateTopic(topic).ConfigureAwait(false);

        if (topicInfo.Existing)
        {
            LogContext.Debug?.Log("Existing topic {Topic} {TopicArn}", topicInfo.EntityName, topicInfo.Arn);
            return topicInfo;
        }

        // Why? I don't know, but damn, it takes two times, or it doesn't catch properly
        topicInfo = await context.CreateTopic(topic).ConfigureAwait(false);

        LogContext.Debug?.Log("Created topic {Topic} {TopicArn}", topicInfo.EntityName, topicInfo.Arn);

        return topicInfo;
    }

    static async Task Declare(ClientContext context, QueueSubscription subscription)
    {
        var created = await context.CreateQueueSubscription(subscription.Source, subscription.Destination).ConfigureAwait(false);
        LogContext.Debug?.Log(created ? "Created subscription {Topic} to {Queue}" : "Existing subscription {Topic} to {Queue}",
            subscription.Source, subscription.Destination);
    }

    static async Task<QueueInfo> Declare(ClientContext context, Queue queue)
    {
        var queueInfo = await context.CreateQueue(queue).ConfigureAwait(false);
        if (queueInfo.Existing)
        {
            LogContext.Debug?.Log("Existing queue {Queue} {QueueArn} {QueueUrl}", queueInfo.EntityName, queueInfo.Arn, queueInfo.Url);
            return queueInfo;
        }

        // Why? I don't know, but damn, it takes two times, or it doesn't catch properly
        queueInfo = await context.CreateQueue(queue).ConfigureAwait(false);

        LogContext.Debug?.Log("Created queue {Queue} {QueueArn} {QueueUrl}", queueInfo.EntityName, queueInfo.Arn, queueInfo.Url);

        return queueInfo;
    }
}
