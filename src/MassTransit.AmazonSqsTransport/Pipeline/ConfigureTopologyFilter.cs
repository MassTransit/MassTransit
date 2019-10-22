namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
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

            var subscriptions = _brokerTopology.QueueSubscriptions.Select(queue => Declare(context, queue));

            await Task.WhenAll(topics.Concat(queues)).ConfigureAwait(false);
            foreach (var task in subscriptions)
                await task;
        }

        async Task DeleteAutoDelete(ClientContext context)
        {
            var topics = _brokerTopology.Topics.Where(x => x.AutoDelete).Select(topic => Delete(context, topic));

            var queues = _brokerTopology.Queues.Where(x => x.AutoDelete).Select(queue => Delete(context, queue));

            await Task.WhenAll(topics.Concat(queues)).ConfigureAwait(false);
        }

        Task Declare(ClientContext context, Topic topic)
        {
            LogContext.Debug?.Log("Create topic {Topic}", topic);

            return context.CreateTopic(topic);
        }

        Task Declare(ClientContext context, QueueSubscription subscription)
        {
            LogContext.Debug?.Log("Binding topic {Topic} to {Queue}", subscription.Source, subscription.Destination);

            return context.CreateQueueSubscription(subscription.Source, subscription.Destination);
        }

        Task Declare(ClientContext context, Queue queue)
        {
            LogContext.Debug?.Log("Create queue {Queue}", queue);

            return context.CreateQueue(queue);
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
