namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Topology;
    using Topology.Builders;
    using Topology.Entities;


    /// <summary>
    /// Configures the broker with the supplied topology once the model is created, to ensure
    /// that the exchanges, queues, and bindings for the model are properly configured in AmazonSQS.
    /// </summary>
    public class ConfigureTopologyFilter<TSettings> :
        IFilter<ClientContext>
        where TSettings : class
    {
        readonly BrokerTopology _brokerTopology;
        readonly TSettings _settings;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
        }

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(async payload =>
            {
                await ConfigureTopology(context).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);
            }, () => new Context()).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);

            if (_settings is ReceiveSettings)
                await DeleteAutoDelete(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        async Task ConfigureTopology(ClientContext context)
        {
            var topics = _brokerTopology.Topics.Select(topic => Declare(context, topic));

            var queues = _brokerTopology.Queues.Select(queue => Declare(context, queue));

            await Task.WhenAll(topics).ConfigureAwait(false);
            await Task.WhenAll(queues).ConfigureAwait(false);

            var subscriptions = _brokerTopology.QueueSubscriptions.Select(subscription => Declare(context, subscription));
            await Task.WhenAll(subscriptions).ConfigureAwait(false);
        }

        async Task DeleteAutoDelete(ClientContext context)
        {
            var topics = _brokerTopology.Topics.Where(x => x.AutoDelete).Select(topic => Delete(context, topic));

            var queues = _brokerTopology.Queues.Where(x => x.AutoDelete).Select(queue => Delete(context, queue));

            await Task.WhenAll(topics.Concat(queues)).ConfigureAwait(false);
        }

        async Task<TopicInfo> Declare(ClientContext context, Topic topic)
        {
            TopicInfo topicInfo = await context.CreateTopic(topic).ConfigureAwait(false);
            topicInfo = await context.CreateTopic(topic).ConfigureAwait(false);

            LogContext.Debug?.Log("Created topic {Topic} {TopicArn}", topicInfo.EntityName, topicInfo.Arn);

            return topicInfo;
        }

        Task Declare(ClientContext context, QueueSubscription subscription)
        {
            LogContext.Debug?.Log("Binding topic {Topic} to {Queue}", subscription.Source, subscription.Destination);

            return context.CreateQueueSubscription(subscription.Source, subscription.Destination);
        }

        async Task<QueueInfo> Declare(ClientContext context, Queue queue)
        {
            // Why? I don't know, but damn, it takes two times, or it doesn't catch properly
            QueueInfo queueInfo = await context.CreateQueue(queue).ConfigureAwait(false);

            queueInfo = await context.CreateQueue(queue).ConfigureAwait(false);

            LogContext.Debug?.Log("Created queue {Queue} {QueueArn} {QueueUrl}", queueInfo.EntityName, queueInfo.Arn, queueInfo.Url);

            return queueInfo;
        }

        Task Delete(ClientContext context, Topic topic)
        {
            LogContext.Debug?.Log("Delete topic {Topic}", topic);

            return context.DeleteTopic(topic);
        }

        Task Delete(ClientContext context, Queue queue)
        {
            LogContext.Debug?.Log("Delete queue {Queue}", queue);

            return context.DeleteQueue(queue);
        }


        class Context :
            ConfigureTopologyContext<TSettings>
        {
        }
    }
}
