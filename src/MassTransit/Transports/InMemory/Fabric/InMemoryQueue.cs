namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using Util;


    public class InMemoryQueue :
        IInMemoryQueue
    {
        readonly TaskCompletionSource<IInMemoryQueueConsumer> _consumer;
        readonly Connectable<IInMemoryQueueConsumer> _consumers;
        readonly ChannelExecutor _executor;
        readonly string _name;

        public InMemoryQueue(string name, int concurrencyLevel)
        {
            _name = name;

            _consumers = new Connectable<IInMemoryQueueConsumer>();
            _consumer = Util.TaskUtil.GetTask<IInMemoryQueueConsumer>();

            _executor = new ChannelExecutor(concurrencyLevel, false);
        }

        public ConnectHandle ConnectConsumer(IInMemoryQueueConsumer consumer)
        {
            try
            {
                var handle = _consumers.Connect(consumer);

                _consumer.SetResult(consumer);

                return handle;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {_name}", exception);
            }
        }

        public Task Deliver(DeliveryContext<InMemoryTransportMessage> context)
        {
            return context.WasAlreadyDelivered(this)
                ? Task.FromResult(false)
                : _executor.Push(() => DispatchMessage(context.Package));
        }

        public ValueTask DisposeAsync()
        {
            return _executor.DisposeAsync();
        }

        async Task DispatchMessage(InMemoryTransportMessage message)
        {
            await _consumer.Task.ConfigureAwait(false);

            try
            {
                await _consumers.ForEachAsync(x => x.Consume(message, CancellationToken.None)).ConfigureAwait(false);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        public override string ToString()
        {
            return $"Queue({_name})";
        }
    }
}
