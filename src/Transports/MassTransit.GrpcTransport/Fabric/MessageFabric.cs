namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Internals.GraphValidation;


    public class MessageFabric :
        IMessageFabric
    {
        readonly int _concurrencyLimit;
        readonly ConcurrentDictionary<string, IGrpcExchange> _exchanges;
        readonly MessageFabricObservable _observers;
        readonly ConcurrentDictionary<string, IGrpcQueue> _queues;

        public MessageFabric(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;

            _observers = new MessageFabricObservable();

            _exchanges = new ConcurrentDictionary<string, IGrpcExchange>(StringComparer.OrdinalIgnoreCase);
            _queues = new ConcurrentDictionary<string, IGrpcQueue>(StringComparer.OrdinalIgnoreCase);
        }

        public void ExchangeDeclare(NodeContext context, string name)
        {
            _exchanges.GetOrAdd(name, x => CreateExchange(context, x));
        }

        public void QueueDeclare(NodeContext context, string name, int? concurrencyLimit = default)
        {
            _queues.GetOrAdd(name, x => CreateQueue(context, x, concurrencyLimit ?? _concurrencyLimit));
        }

        public void ExchangeBind(NodeContext context, string source, string destination, string routingKey)
        {
            if (source.Equals(destination))
                throw new ArgumentException("The source and destination exchange cannot be the same");

            var sourceExchange = _exchanges.GetOrAdd(source, name => CreateExchange(context, name));

            var destinationExchange = _exchanges.GetOrAdd(destination, name => CreateExchange(context, name));

            ValidateBinding(destinationExchange, sourceExchange);

            _observers.ExchangeBindingCreated(context, source, destination, routingKey);

            sourceExchange.Connect(destinationExchange);
        }

        public void QueueBind(NodeContext context, string source, string destination, string routingKey)
        {
            var sourceExchange = _exchanges.GetOrAdd(source, name => CreateExchange(context, name));

            var destinationQueue = _queues.GetOrAdd(destination, name => CreateQueue(context, name, _concurrencyLimit));

            ValidateBinding(destinationQueue, sourceExchange);

            _observers.QueueBindingCreated(context, source, destination, routingKey);

            sourceExchange.Connect(destinationQueue);
        }

        public IGrpcQueue GetQueue(NodeContext context, string name)
        {
            return _queues.GetOrAdd(name, x => CreateQueue(context, name, _concurrencyLimit));
        }

        public IGrpcExchange GetExchange(NodeContext context, string name)
        {
            return _exchanges.GetOrAdd(name, x => CreateExchange(context, x));
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("messageFabric");
            foreach (KeyValuePair<string, IGrpcExchange> exchange in _exchanges)
            {
                var exchangeScope = scope.CreateScope("exchange");
                exchangeScope.Add("name", exchange.Key);
                foreach (IMessageSink<GrpcTransportMessage> sink in exchange.Value.Sinks)
                    exchangeScope.CreateScope("sink").Add("name", sink.ToString());
            }

            foreach (KeyValuePair<string, IGrpcQueue> exchange in _queues)
            {
                var exchangeScope = scope.CreateScope("queue");
                exchangeScope.Add("name", exchange.Key);
            }
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var queue in _queues.Values)
                await queue.DisposeAsync().ConfigureAwait(false);
        }

        public ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver observer)
        {
            return _observers.Connect(observer);
        }

        GrpcExchange CreateExchange(NodeContext context, string name)
        {
            _observers.ExchangeDeclared(context, name);

            return new GrpcExchange(name);
        }

        GrpcQueue CreateQueue(NodeContext context, string name, int concurrencyLimit)
        {
            _observers.QueueDeclared(context, name);

            return new GrpcQueue(_observers, name, concurrencyLimit == 0 ? _concurrencyLimit : concurrencyLimit);
        }

        void ValidateBinding(IMessageSink<GrpcTransportMessage> destination, IGrpcExchange sourceExchange)
        {
            try
            {
                var graph = new DependencyGraph<IMessageSink<GrpcTransportMessage>>(_exchanges.Count + 1);
                var exchanges = new List<IGrpcExchange>(_exchanges.Values);
                foreach (var exchange in exchanges)
                {
                    var sinks = new List<IMessageSink<GrpcTransportMessage>>(exchange.Sinks);
                    foreach (IMessageSink<GrpcTransportMessage> sink in sinks)
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
