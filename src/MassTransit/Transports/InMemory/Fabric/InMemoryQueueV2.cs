namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;

    public class InMemoryQueueV2 :
        IInMemoryQueue
    {
        readonly CancellationTokenSource _cancellationToken;
        readonly TaskCompletionSource<IInMemoryQueueConsumer> _consumer;
        readonly Connectable<IInMemoryQueueConsumer> _consumers;
        readonly string _name;
        readonly int _concurrencyLevel;
        int _queueDepth;
        int _readersStarted;
        readonly Channel<InMemoryTransportMessage> _channel = Channel.CreateUnbounded<InMemoryTransportMessage>(new UnboundedChannelOptions() {
            AllowSynchronousContinuations = false
        });

        public InMemoryQueueV2(string name, int concurrencyLevel)
        {
            _name = name;
            _concurrencyLevel = concurrencyLevel;
            _cancellationToken = new CancellationTokenSource();

            _consumers = new Connectable<IInMemoryQueueConsumer>();
            _consumer = Util.TaskUtil.GetTask<IInMemoryQueueConsumer>();
            _cancellationToken.Token.Register(() => {
                _channel.Writer.Complete();
                _consumer.TrySetCanceled();
            });
        }

        public ConnectHandle ConnectConsumer(IInMemoryQueueConsumer consumer)
        {
            try
            {
                var handle = _consumers.Connect(consumer);

                _consumer.SetResult(consumer);
                CreateChannelReaders();
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
            return _channel.Writer.WriteAsync(context.Package, _cancellationToken.Token).AsTask();
        }

        void CreateChannelReaders() {
            if (Interlocked.CompareExchange(ref _readersStarted, 1, 1) == 1) {
                return;
            }
            for (var i = 0; i < _concurrencyLevel; i++) {
                Task.Run(ReadNextMessage);
            }
        }

        async Task ReadNextMessage() {
            await _consumer.Task.ConfigureAwait(false);
            while (await _channel.Reader.WaitToReadAsync()) {
                var message = await _channel.Reader.ReadAsync();
                Interlocked.Increment(ref _queueDepth);
                if (_cancellationToken.IsCancellationRequested)
                    return;
                try
                {
                    await _consumers.ForEachAsync(x => x.Consume(message, _cancellationToken.Token)).ConfigureAwait(false);
                }
                catch
                {
                }
                finally
                {
                    Interlocked.Decrement(ref _queueDepth);
                }
            }
        }

        public override string ToString()
        {
            return $"Queue({_name})";
        }
    }
}
