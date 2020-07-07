namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using MassTransit.Util;

    public class InMemoryQueueV2 :
        IInMemoryQueue
    {
        readonly CancellationTokenSource _cancellationToken;
        readonly TaskCompletionSource<IInMemoryQueueConsumer> _consumer;
        readonly Connectable<IInMemoryQueueConsumer> _consumers;
        readonly string _name;
        int _queueDepth;
        readonly ChannelExecutor _channelExecutor;

        public InMemoryQueueV2(string name, int concurrencyLevel)
        {
            _name = name;
            _cancellationToken = new CancellationTokenSource();

            _consumers = new Connectable<IInMemoryQueueConsumer>();
            _consumer = Util.TaskUtil.GetTask<IInMemoryQueueConsumer>();
            _channelExecutor = new ChannelExecutor(concurrencyLevel, false);
            _cancellationToken.Token.Register(async () => {
                await _channelExecutor.DisposeAsync();
                _consumer.TrySetCanceled();
            });
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
            if (context.WasAlreadyDelivered(this))
                return Task.FromResult(false);
            return _channelExecutor.Push(() => ReadNextMessage(context.Package));
        }

        async Task ReadNextMessage(InMemoryTransportMessage message) {
            await _consumer.Task.ConfigureAwait(false);
            try
            {
                await _consumers.ForEachAsync(x => x.Consume(message, _cancellationToken.Token))
                    .ConfigureAwait(false);
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref _queueDepth);
            }
        }

        public override string ToString()
        {
            return $"Queue({_name})";
        }
    }
}
