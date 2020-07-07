namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using Internals.GraphValidation;


    public class MessageFabric :
        IMessageFabric
    {
        readonly ConcurrentDictionary<string, IInMemoryExchange> _exchanges;
        readonly ConcurrentDictionary<string, IInMemoryQueue> _queues;
        int _concurrencyLimit;

        public MessageFabric(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;

            _exchanges = new ConcurrentDictionary<string, IInMemoryExchange>(StringComparer.OrdinalIgnoreCase);
            _queues = new ConcurrentDictionary<string, IInMemoryQueue>(StringComparer.OrdinalIgnoreCase);
        }

        public Task Send(string exchangeName, InMemoryTransportMessage message)
        {
            var exchange = new InMemoryExchange(exchangeName);

            var deliveryContext = new InMemoryDeliveryContext(message);
            return exchange.Deliver(deliveryContext);
        }

        public void ExchangeDeclare(string name)
        {
            _exchanges.GetOrAdd(name, x => new InMemoryExchange(x));
        }

        public void QueueDeclare(string name, int concurrencyLimit)
        {
            _queues.GetOrAdd(name, x => new InMemoryQueueV2(x, concurrencyLimit == 0 ? _concurrencyLimit : concurrencyLimit));
        }

        public void ExchangeBind(string source, string destination)
        {
            if (source.Equals(destination))
                throw new ArgumentException("The source and destination exchange cannot be the same");

            var sourceExchange = _exchanges.GetOrAdd(source, x => new InMemoryExchange(x));

            var destinationExchange = _exchanges.GetOrAdd(destination, x => new InMemoryExchange(x));

            ValidateBinding(destinationExchange, sourceExchange);

            sourceExchange.Connect(destinationExchange);
        }

        public void QueueBind(string source, string destination)
        {
            var sourceExchange = _exchanges.GetOrAdd(source, x => new InMemoryExchange(x));

            var destinationQueue = _queues.GetOrAdd(destination, x => new InMemoryQueueV2(destination, _concurrencyLimit));

            ValidateBinding(destinationQueue, sourceExchange);

            sourceExchange.Connect(destinationQueue);
        }

        public IInMemoryQueue GetQueue(string name)
        {
            return _queues.GetOrAdd(name, x => new InMemoryQueueV2(x, _concurrencyLimit));
        }

        public IInMemoryExchange GetExchange(string name)
        {
            return _exchanges.GetOrAdd(name, x => new InMemoryExchange(x));
        }

        public int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("messageFabric");
            foreach (KeyValuePair<string, IInMemoryExchange> exchange in _exchanges)
            {
                var exchangeScope = scope.CreateScope("exchange");
                exchangeScope.Add("name", exchange.Key);
                foreach (IMessageSink<InMemoryTransportMessage> sink in exchange.Value.Sinks)
                    exchangeScope.CreateScope("sink").Add("name", sink.ToString());
            }

            foreach (KeyValuePair<string, IInMemoryQueue> exchange in _queues)
            {
                var exchangeScope = scope.CreateScope("queue");
                exchangeScope.Add("name", exchange.Key);
            }
        }

        void ValidateBinding(IMessageSink<InMemoryTransportMessage> destination, IInMemoryExchange sourceExchange)
        {
            try
            {
                var graph = new DependencyGraph<IMessageSink<InMemoryTransportMessage>>(_exchanges.Count + 1);
                var exchanges = new List<IInMemoryExchange>(_exchanges.Values);
                foreach (var exchange in exchanges)
                {
                    var sinks = new List<IMessageSink<InMemoryTransportMessage>>(exchange.Sinks);
                    foreach (IMessageSink<InMemoryTransportMessage> sink in sinks)
                        graph.Add(sink, exchange);
                }

                graph.Add(destination, sourceExchange);

                graph.EnsureGraphIsAcyclic();
            }
            catch (CyclicGraphException exception)
            {
                throw new InvalidOperationException("The exchange binding would create a cycle in the messaging fabric.", exception);
            }
        }
    }
}
