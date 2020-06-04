namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using System.Threading;
    using Entities;
    using MassTransit.Topology.Entities;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


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

        public TopicHandle CreateTopic(TopicDescription topicDescription)
        {
            var exchange = new TopicEntity(GetNextId(), topicDescription);

            return Topics.GetOrAdd(exchange);
        }

        public SubscriptionHandle CreateSubscription(TopicHandle topic, SubscriptionDescription subscriptionDescription, RuleDescription rule, Filter filter)
        {
            var topicEntity = Topics.Get(topic);

            var subscriptionEntity = new SubscriptionEntity(GetNextId(), topicEntity, subscriptionDescription, rule, filter);

            return Subscriptions.GetOrAdd(subscriptionEntity);
        }

        public QueueHandle CreateQueue(QueueDescription queueDescription)
        {
            var queue = new QueueEntity(GetNextId(), queueDescription);

            return Queues.GetOrAdd(queue);
        }

        public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, SubscriptionDescription subscriptionDescription,
            RuleDescription rule,
            Filter filter)
        {
            var topicEntity = Topics.Get(exchange);

            var queueEntity = Queues.Get(queue);

            if (topicEntity.TopicDescription.EnablePartitioning)
                queueEntity.QueueDescription.EnablePartitioning = true;

            var binding = new QueueSubscriptionEntity(GetNextId(), GetNextId(), topicEntity, queueEntity, subscriptionDescription, rule, filter);

            return QueueSubscriptions.GetOrAdd(binding);
        }

        public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SubscriptionDescription subscriptionDescription)
        {
            var sourceEntity = Topics.Get(source);

            var destinationEntity = Topics.Get(destination);

            if (sourceEntity.TopicDescription.EnablePartitioning)
                destinationEntity.TopicDescription.EnablePartitioning = true;

            var subscriptionEntity = new TopicSubscriptionEntity(GetNextId(), GetNextId(), sourceEntity, destinationEntity, subscriptionDescription);

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
