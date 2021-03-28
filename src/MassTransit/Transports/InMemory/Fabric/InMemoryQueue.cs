namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
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

                _consumer.TrySetResult(consumer);

                return new ConsumerHandle(this, handle);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {_name}", exception);
            }
        }

        public Task Deliver(DeliveryContext<InMemoryTransportMessage> context)
        {
            return context.WasAlreadyDelivered(this)
                ? Task.CompletedTask
                : context.Message.Delay > TimeSpan.Zero
                    ? DeliverWithDelay(context)
                    : _executor.Push(() => DispatchMessage(context), context.CancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _executor.DisposeAsync();
        }

        async Task DeliverWithDelay(DeliveryContext<InMemoryTransportMessage> context)
        {
            await Task.Delay(context.Message.Delay.Value, context.CancellationToken).ConfigureAwait(false);

            await _executor.Push(() => DispatchMessage(context), context.CancellationToken).ConfigureAwait(false);
        }

        async Task DispatchMessage(DeliveryContext<InMemoryTransportMessage> context)
        {
            await _consumer.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);

            try
            {
                await _consumers.ForEachAsync(x => x.Consume(context.Message, context.CancellationToken)).ConfigureAwait(false);
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


        class ConsumerHandle :
            ConnectHandle
        {
            readonly ConnectHandle _handle;
            readonly InMemoryQueue _queue;

            public ConsumerHandle(InMemoryQueue queue, ConnectHandle handle)
            {
                _queue = queue;
                _handle = handle;
            }

            public void Dispose()
            {
                _handle.Dispose();
            }

            public void Disconnect()
            {
                _handle.Disconnect();
            }
        }
    }
}
