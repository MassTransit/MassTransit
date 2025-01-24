#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using System.Threading;
    using MassTransit.Topology;


    public abstract class BrokerTopologyBuilder
    {
        readonly NamedEntityCollection<QueueEntity, QueueHandle> _queues;
        readonly EntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle> _queueSubscriptions;
        readonly NamedEntityCollection<TopicEntity, TopicHandle> _topics;
        readonly EntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle> _topicSubscriptions;
        long _nextId;

        protected BrokerTopologyBuilder()
        {
            _topics = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            _queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            _topicSubscriptions = new EntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle>(TopicSubscriptionEntity.EntityComparer);
            _queueSubscriptions = new EntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle>(QueueSubscriptionEntity.EntityComparer);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        public TopicHandle CreateTopic(string name)
        {
            var id = GetNextId();

            var exchange = new TopicEntity(id, name);

            return _topics.GetOrAdd(exchange);
        }

        public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SqlSubscriptionType subscriptionType,
            string? routingKey)
        {
            var id = GetNextId();

            var sourceExchange = _topics.Get(source);

            var destinationExchange = _topics.Get(destination);

            var binding = new TopicSubscriptionEntity(id, sourceExchange, destinationExchange, subscriptionType, routingKey);

            return _topicSubscriptions.GetOrAdd(binding);
        }

        public QueueHandle CreateQueue(string name, TimeSpan? autoDeleteOnIdle = null, int? maxDeliveryCount = null)
        {
            var id = GetNextId();

            var queue = new QueueEntity(id, name, autoDeleteOnIdle, maxDeliveryCount);

            return _queues.GetOrAdd(queue);
        }

        public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue, SqlSubscriptionType subscriptionType, string? routingKey)
        {
            var id = GetNextId();

            var exchangeEntity = _topics.Get(topic);

            var queueEntity = _queues.Get(queue);

            var binding = new QueueSubscriptionEntity(id, exchangeEntity, queueEntity, subscriptionType, routingKey);

            return _queueSubscriptions.GetOrAdd(binding);
        }

        public BrokerTopology BuildBrokerTopology()
        {
            return new SqlBrokerTopology(_topics, _topicSubscriptions, _queues, _queueSubscriptions);
        }
    }
}
