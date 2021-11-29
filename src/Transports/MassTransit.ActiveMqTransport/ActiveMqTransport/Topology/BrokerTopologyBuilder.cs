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

        public QueueHandle CreateQueue(string name, bool autoDelete)
        {
            var id = GetNextId();

            var queue = new QueueEntity(id, name, autoDelete);

            return Queues.GetOrAdd(queue);
        }

        public ConsumerHandle BindConsumer(TopicHandle topic, QueueHandle queue, string selector)
        {
            var id = GetNextId();

            var exchangeEntity = Topics.Get(topic);

            var queueEntity = Queues.Get(queue);

            var binding = new ConsumerEntity(id, exchangeEntity, queueEntity, selector);

            return Consumers.GetOrAdd(binding);
        }
    }
}
