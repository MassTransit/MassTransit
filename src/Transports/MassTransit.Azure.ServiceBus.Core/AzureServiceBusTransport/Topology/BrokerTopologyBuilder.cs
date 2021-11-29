namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System.Threading;
    using Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Topology;


    public class BrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        long _nextId;

        public BrokerTopologyBuilder()
        {
            Topics = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.EntityComparer, QueueEntity.NameComparer);

            Subscriptions = new NamedEntityCollection<SubscriptionEntity, SubscriptionHandle>(SubscriptionEntity.EntityComparer,
                SubscriptionEntity.NameComparer);

            QueueSubscriptions = new NamedEntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle>(QueueSubscriptionEntity.EntityComparer,
                QueueSubscriptionEntity.NameComparer);

            TopicSubscriptions = new NamedEntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle>(TopicSubscriptionEntity.EntityComparer,
                TopicSubscriptionEntity.NameComparer);
        }

        protected EntityCollection<SubscriptionEntity, SubscriptionHandle> Subscriptions { get; }
        protected NamedEntityCollection<TopicEntity, TopicHandle> Topics { get; }
        protected EntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle> QueueSubscriptions { get; }
        protected EntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle> TopicSubscriptions { get; }
        protected NamedEntityCollection<QueueEntity, QueueHandle> Queues { get; }

        public TopicHandle CreateTopic(CreateTopicOptions createTopicOptions)
        {
            var exchange = new TopicEntity(GetNextId(), createTopicOptions);

            return Topics.GetOrAdd(exchange);
        }

        public SubscriptionHandle CreateSubscription(TopicHandle topic, CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule,
            RuleFilter filter)
        {
            var topicEntity = Topics.Get(topic);

            var subscriptionEntity = new SubscriptionEntity(GetNextId(), topicEntity, createSubscriptionOptions, rule, filter);

            return Subscriptions.GetOrAdd(subscriptionEntity);
        }

        public QueueHandle CreateQueue(CreateQueueOptions createQueueOptions)
        {
            var queue = new QueueEntity(GetNextId(), createQueueOptions);

            return Queues.GetOrAdd(queue);
        }

        public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, CreateSubscriptionOptions createSubscriptionOptions,
            CreateRuleOptions rule, RuleFilter filter)
        {
            var topicEntity = Topics.Get(exchange);

            var queueEntity = Queues.Get(queue);

            if (topicEntity.CreateTopicOptions.EnablePartitioning)
                queueEntity.CreateQueueOptions.EnablePartitioning = true;

            var binding = new QueueSubscriptionEntity(GetNextId(), GetNextId(), topicEntity, queueEntity, createSubscriptionOptions, rule, filter);

            return QueueSubscriptions.GetOrAdd(binding);
        }

        public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, CreateSubscriptionOptions createSubscriptionOptions)
        {
            var sourceEntity = Topics.Get(source);

            var destinationEntity = Topics.Get(destination);

            if (sourceEntity.CreateTopicOptions.EnablePartitioning)
                destinationEntity.CreateTopicOptions.EnablePartitioning = true;

            var subscriptionEntity = new TopicSubscriptionEntity(GetNextId(), GetNextId(), sourceEntity, destinationEntity, createSubscriptionOptions);

            return TopicSubscriptions.GetOrAdd(subscriptionEntity);
        }

        public BrokerTopology BuildBrokerTopology()
        {
            return new ServiceBusBrokerTopology(Topics, Subscriptions, Queues, QueueSubscriptions, TopicSubscriptions);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }
    }
}
