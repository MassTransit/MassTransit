namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Threading;
    using MassTransit.Topology;


    public abstract class BrokerTopologyBuilder
    {
        readonly EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle> _exchangeBindings;
        readonly NamedEntityCollection<ExchangeEntity, ExchangeHandle> _exchanges;
        readonly EntityCollection<QueueBindingEntity, QueueBindingHandle> _queueBindings;
        readonly NamedEntityCollection<QueueEntity, QueueHandle> _queues;
        long _nextId;

        protected BrokerTopologyBuilder()
        {
            _exchanges = new NamedEntityCollection<ExchangeEntity, ExchangeHandle>(ExchangeEntity.EntityComparer, ExchangeEntity.NameComparer);
            _queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            _exchangeBindings = new EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle>(ExchangeBindingEntity.EntityComparer);
            _queueBindings = new EntityCollection<QueueBindingEntity, QueueBindingHandle>(QueueBindingEntity.EntityComparer);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        public ExchangeHandle ExchangeDeclare(string name, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var exchange = new ExchangeEntity(id, name, type, durable, autoDelete, arguments);

            return _exchanges.GetOrAdd(exchange);
        }

        public ExchangeBindingHandle ExchangeBind(ExchangeHandle source, ExchangeHandle destination, string routingKey, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var sourceExchange = _exchanges.Get(source);

            var destinationExchange = _exchanges.Get(destination);

            var binding = new ExchangeBindingEntity(id, sourceExchange, destinationExchange, routingKey, arguments);

            return _exchangeBindings.GetOrAdd(binding);
        }

        public QueueHandle QueueDeclare(string name, bool durable, bool autoDelete, bool exclusive, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var queue = new QueueEntity(id, name, durable, autoDelete, exclusive, arguments);

            return _queues.GetOrAdd(queue);
        }

        public QueueBindingHandle QueueBind(ExchangeHandle exchange, QueueHandle queue, string routingKey, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var exchangeEntity = _exchanges.Get(exchange);

            var queueEntity = _queues.Get(queue);

            var binding = new QueueBindingEntity(id, exchangeEntity, queueEntity, routingKey, arguments);

            return _queueBindings.GetOrAdd(binding);
        }

        public BrokerTopology BuildBrokerTopology()
        {
            return new RabbitMqBrokerTopology(_exchanges, _exchangeBindings, _queues, _queueBindings);
        }
    }
}
