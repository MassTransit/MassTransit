namespace MassTransit.Transports.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Internals.GraphValidation;
    using Middleware;


    public class MessageFabric<TContext, T> :
        Supervisor,
        IMessageFabric<TContext, T>
        where T : class
        where TContext : class
    {
        readonly ConcurrentDictionary<string, IMessageExchange<T>> _exchanges;
        readonly MessageFabricObservable<TContext> _observers;
        readonly ConcurrentDictionary<string, IMessageQueue<TContext, T>> _queues;

        public MessageFabric()
        {
            _observers = new MessageFabricObservable<TContext>();

            _exchanges = new ConcurrentDictionary<string, IMessageExchange<T>>(StringComparer.OrdinalIgnoreCase);
            _queues = new ConcurrentDictionary<string, IMessageQueue<TContext, T>>(StringComparer.OrdinalIgnoreCase);
        }

        public void ExchangeDeclare(TContext context, string name, ExchangeType exchangeType)
        {
            GetOrAddExchange(context, name, exchangeType);
        }

        public void ExchangeBind(TContext context, string source, string destination, string routingKey)
        {
            if (source.Equals(destination))
                throw new ArgumentException("The source and destination exchange cannot be the same");

            IMessageExchange<T> sourceExchange = GetOrAddExchange(context, source, ExchangeType.FanOut);

            IMessageExchange<T> destinationExchange = GetOrAddExchange(context, destination, ExchangeType.FanOut);

            ValidateBinding(destinationExchange, sourceExchange);

            _observers.ExchangeBindingCreated(context, source, destination, routingKey);

            sourceExchange.Connect(destinationExchange, routingKey);
        }

        public void QueueDeclare(TContext context, string name)
        {
            GetOrAddQueue(context, name);
        }

        public void QueueBind(TContext context, string source, string destination)
        {
            IMessageExchange<T> sourceExchange = GetOrAddExchange(context, source, ExchangeType.FanOut);

            IMessageQueue<TContext, T> destinationQueue = GetOrAddQueue(context, destination);

            ValidateBinding(destinationQueue, sourceExchange);

            _observers.QueueBindingCreated(context, source, destination);

            sourceExchange.Connect(destinationQueue, default);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("messageFabric");
            foreach (IMessageExchange<T> exchange in _exchanges.Values)
                exchange.Probe(scope);

            foreach (IMessageQueue<TContext, T> queue in _queues.Values)
                queue.Probe(scope);
        }

        public IMessageExchange<T> GetExchange(TContext context, string name, ExchangeType exchangeType)
        {
            return GetOrAddExchange(context, name, exchangeType);
        }

        public IMessageQueue<TContext, T> GetQueue(TContext context, string name)
        {
            return GetOrAddQueue(context, name);
        }

        public ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver<TContext> observer)
        {
            return _observers.Connect(observer);
        }

        IMessageQueue<TContext, T> GetOrAddQueue(TContext context, string name)
        {
            MessageQueue<TContext, T> created = null;
            IMessageQueue<TContext, T> queue = _queues.GetOrAdd(name, x =>
            {
                created = new MessageQueue<TContext, T>(_observers, name);

                return created;
            });

            if (created != null && queue == created)
            {
                Add(created);

                _observers.QueueDeclared(context, name);
            }

            return queue;
        }

        IMessageExchange<T> GetOrAddExchange(TContext context, string name, ExchangeType exchangeType)
        {
            IMessageExchange<T> created = null;
            IMessageExchange<T> exchange = _exchanges.GetOrAdd(name, x =>
            {
                created = exchangeType switch
                {
                    ExchangeType.FanOut => new MessageFanOutExchange<T>(name),
                    ExchangeType.Direct => new MessageDirectExchange<T>(name),
                    ExchangeType.Topic => new MessageTopicExchange<T>(name),
                    _ => throw new ArgumentException($"Unsupported exchange type: {exchangeType}", nameof(exchangeType))
                };

                return created;
            });

            if (created != null && exchange == created)
                _observers.ExchangeDeclared(context, name, exchangeType);

            return exchange;
        }

        void ValidateBinding(IMessageSink<T> destination, IMessageSink<T> sourceExchange)
        {
            try
            {
                var graph = new DependencyGraph<IMessageSink<T>>(_exchanges.Count + 1);
                var exchanges = new List<IMessageExchange<T>>(_exchanges.Values);
                foreach (IMessageExchange<T> exchange in exchanges)
                {
                    var sinks = new List<IMessageSink<T>>(exchange.Sinks);
                    foreach (IMessageSink<T> sink in sinks)
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
