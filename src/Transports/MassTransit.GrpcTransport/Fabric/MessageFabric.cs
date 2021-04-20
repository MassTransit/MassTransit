namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes;
    using Internals.GraphValidation;


    public class MessageFabric :
        IMessageFabric
    {
        readonly ConcurrentDictionary<string, IGrpcExchange> _exchanges;
        readonly MessageFabricObservable _observers;
        readonly ConcurrentDictionary<string, IGrpcQueue> _queues;

        public MessageFabric()
        {
            _observers = new MessageFabricObservable();

            _exchanges = new ConcurrentDictionary<string, IGrpcExchange>(StringComparer.OrdinalIgnoreCase);
            _queues = new ConcurrentDictionary<string, IGrpcQueue>(StringComparer.OrdinalIgnoreCase);
        }

        public void ExchangeDeclare(NodeContext context, string name, ExchangeType exchangeType)
        {
            _exchanges.GetOrAdd(name, x => CreateExchange(context, x, exchangeType));
        }

        public void ExchangeBind(NodeContext context, string source, string destination, string routingKey)
        {
            if (source.Equals(destination))
                throw new ArgumentException("The source and destination exchange cannot be the same");

            var sourceExchange = _exchanges.GetOrAdd(source, name => CreateExchange(context, name, ExchangeType.FanOut));

            var destinationExchange = _exchanges.GetOrAdd(destination, name => CreateExchange(context, name, ExchangeType.FanOut));

            ValidateBinding(destinationExchange, sourceExchange);

            _observers.ExchangeBindingCreated(context, source, destination, routingKey);

            sourceExchange.Connect(destinationExchange, routingKey);
        }

        public void QueueDeclare(NodeContext context, string name)
        {
            _queues.GetOrAdd(name, x => CreateQueue(context, x));
        }

        public void QueueBind(NodeContext context, string source, string destination)
        {
            var sourceExchange = _exchanges.GetOrAdd(source, name => CreateExchange(context, name, ExchangeType.FanOut));

            var destinationQueue = _queues.GetOrAdd(destination, name => CreateQueue(context, name));

            ValidateBinding(destinationQueue, sourceExchange);

            _observers.QueueBindingCreated(context, source, destination);

            sourceExchange.Connect(destinationQueue, default);
        }

        public IGrpcExchange GetExchange(NodeContext context, string name, ExchangeType exchangeType)
        {
            return _exchanges.GetOrAdd(name, x => CreateExchange(context, x, exchangeType));
        }

        public IGrpcQueue GetQueue(NodeContext context, string name)
        {
            return _queues.GetOrAdd(name, x => CreateQueue(context, name));
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

        IGrpcExchange CreateExchange(NodeContext context, string name, ExchangeType exchangeType)
        {
            _observers.ExchangeDeclared(context, name, exchangeType);

            return exchangeType switch
            {
                ExchangeType.FanOut => new GrpcFanOutExchange(name),
                ExchangeType.Direct => new GrpcDirectExchange(name),
                _ => throw new ArgumentException($"Unsupported exchange type: {exchangeType}", nameof(exchangeType))
            };
        }

        IGrpcQueue CreateQueue(NodeContext context, string name)
        {
            _observers.QueueDeclared(context, name);

            return new GrpcQueue(_observers, name);
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
