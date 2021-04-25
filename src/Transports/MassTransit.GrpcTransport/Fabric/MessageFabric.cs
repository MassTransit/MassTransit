namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Internals.GraphValidation;


    public class MessageFabric :
        Supervisor,
        IMessageFabric
    {
        readonly ConcurrentDictionary<string, IMessageExchange> _exchanges;
        readonly MessageFabricObservable _observers;
        readonly ConcurrentDictionary<string, IMessageQueue> _queues;

        public MessageFabric()
        {
            _observers = new MessageFabricObservable();

            _exchanges = new ConcurrentDictionary<string, IMessageExchange>(StringComparer.OrdinalIgnoreCase);
            _queues = new ConcurrentDictionary<string, IMessageQueue>(StringComparer.OrdinalIgnoreCase);
        }

        public void ExchangeDeclare(NodeContext context, string name, ExchangeType exchangeType)
        {
            GetOrAddExchange(context, name, exchangeType);
        }

        public void ExchangeBind(NodeContext context, string source, string destination, string routingKey)
        {
            if (source.Equals(destination))
                throw new ArgumentException("The source and destination exchange cannot be the same");

            var sourceExchange = GetOrAddExchange(context, source, ExchangeType.FanOut);

            var destinationExchange = GetOrAddExchange(context, destination, ExchangeType.FanOut);

            ValidateBinding(destinationExchange, sourceExchange);

            _observers.ExchangeBindingCreated(context, source, destination, routingKey);

            sourceExchange.Connect(destinationExchange, routingKey);
        }

        public void QueueDeclare(NodeContext context, string name)
        {
            GetOrAddQueue(context, name);
        }

        public void QueueBind(NodeContext context, string source, string destination)
        {
            var sourceExchange = GetOrAddExchange(context, source, ExchangeType.FanOut);

            var destinationQueue = GetOrAddQueue(context, destination);

            ValidateBinding(destinationQueue, sourceExchange);

            _observers.QueueBindingCreated(context, source, destination);

            sourceExchange.Connect(destinationQueue, default);
        }

        public IMessageExchange GetExchange(NodeContext context, string name, ExchangeType exchangeType)
        {
            return GetOrAddExchange(context, name, exchangeType);
        }

        public IMessageQueue GetQueue(NodeContext context, string name)
        {
            return GetOrAddQueue(context, name);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("messageFabric");
            foreach (var exchange in _exchanges.Values)
                exchange.Probe(scope);

            foreach (var queue in _queues.Values)
                queue.Probe(scope);
        }

        public ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver observer)
        {
            return _observers.Connect(observer);
        }

        IMessageQueue GetOrAddQueue(NodeContext context, string name)
        {
            MessageQueue created = null;
            var queue = _queues.GetOrAdd(name, x =>
            {
                created = new MessageQueue(_observers, name);

                return created;
            });

            if (created != null && queue == created)
            {
                Add(created);

                _observers.QueueDeclared(context, name);
            }

            return queue;
        }

        IMessageExchange GetOrAddExchange(NodeContext context, string name, ExchangeType exchangeType)
        {
            IMessageExchange created = null;
            var exchange = _exchanges.GetOrAdd(name, x =>
            {
                created = exchangeType switch
                {
                    ExchangeType.FanOut => new MessageFanOutExchange(name),
                    ExchangeType.Direct => new MessageDirectExchange(name),
                    ExchangeType.Topic => new MessageTopicExchange(name),
                    _ => throw new ArgumentException($"Unsupported exchange type: {exchangeType}", nameof(exchangeType))
                };

                return created;
            });

            if (created != null && exchange == created)
                _observers.ExchangeDeclared(context, name, exchangeType);

            return exchange;
        }

        void ValidateBinding(IMessageSink<GrpcTransportMessage> destination, IMessageExchange sourceExchange)
        {
            try
            {
                var graph = new DependencyGraph<IMessageSink<GrpcTransportMessage>>(_exchanges.Count + 1);
                var exchanges = new List<IMessageExchange>(_exchanges.Values);
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
