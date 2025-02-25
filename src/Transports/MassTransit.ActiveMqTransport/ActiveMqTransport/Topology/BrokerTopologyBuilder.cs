namespace MassTransit.ActiveMqTransport.Topology
{
    using System.Threading;
    using MassTransit.Topology;


    public abstract class BrokerTopologyBuilder
    {
        protected readonly NamedEntityCollection<ConsumerEntity, ConsumerHandle> Consumers;
        protected readonly NamedEntityCollection<QueueEntity, QueueHandle> Queues;
        protected readonly NamedEntityCollection<TopicEntity, TopicHandle> Topics;
        long _nextId;

        protected BrokerTopologyBuilder()
        {
            Topics = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            Consumers = new NamedEntityCollection<ConsumerEntity, ConsumerHandle>(ConsumerEntity.EntityComparer, ConsumerEntity.NameComparer);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        public TopicHandle CreateTopic(string name, bool durable, bool autoDelete)
        {
            var id = GetNextId();

            var exchange = new TopicEntity(id, name, durable, autoDelete);

            return Topics.GetOrAdd(exchange);
        }

        public QueueHandle CreateQueue(string name, bool durable, bool autoDelete)
        {
            var id = GetNextId();

            var queue = new QueueEntity(id, name, durable, autoDelete);

            return Queues.GetOrAdd(queue);
        }

        public ConsumerHandle BindConsumer(TopicHandle topic, QueueHandle queue, string selector, string consumerName = null, bool shared = false)
        {
            var id = GetNextId();

            var exchangeEntity = Topics.Get(topic);

            var queueEntity = queue != null ? Queues.Get(queue) : null;

            var binding = new ConsumerEntity(id, exchangeEntity, queueEntity, selector, consumerName, shared);

            return Consumers.GetOrAdd(binding);
        }
    }
}
