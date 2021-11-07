namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class InMemoryExchange :
        IInMemoryExchange,
        IDisposable
    {
        readonly HashSet<IMessageSink<InMemoryTransportMessage>> _sinks;
        readonly SemaphoreSlim _semaphoreSlim;

        public InMemoryExchange(string name)
        {
            Name = name;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _sinks = new HashSet<IMessageSink<InMemoryTransportMessage>>();
        }

        public string Name { get; }

        public async Task Deliver(DeliveryContext<InMemoryTransportMessage> context)
        {
            foreach (IMessageSink<InMemoryTransportMessage> sink in _sinks)
            {
                if (context.WasAlreadyDelivered(sink))
                    continue;

                await sink.Deliver(context).ConfigureAwait(false);

                context.Delivered(sink);
            }
        }

        public void Connect(IMessageSink<InMemoryTransportMessage> sink)
        {
            _semaphoreSlim.Wait();
            try
            {
                _sinks.Add(sink);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public IEnumerable<IMessageSink<InMemoryTransportMessage>> Sinks => _sinks;

        public Task Send(InMemoryTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new InMemoryDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public override string ToString()
        {
            return $"Exchange({Name})";
        }

        public void Dispose()
        {
            _semaphoreSlim?.Dispose();
        }
    }
}
