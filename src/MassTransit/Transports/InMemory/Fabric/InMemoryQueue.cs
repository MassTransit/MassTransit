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
        readonly CancellationTokenSource _cancellationToken;
        readonly TaskCompletionSource<IInMemoryQueueConsumer> _consumer;
        readonly string _name;
        readonly LimitedConcurrencyLevelTaskScheduler _scheduler;
        readonly Connectable<IInMemoryQueueConsumer> _consumers;
        int _queueDepth;

        public InMemoryQueue(string name, int concurrencyLevel)
        {
            _name = name;
            _scheduler = new LimitedConcurrencyLevelTaskScheduler(concurrencyLevel);
            _cancellationToken = new CancellationTokenSource();

            _consumers = new Connectable<IInMemoryQueueConsumer>();
            _consumer = Util.TaskUtil.GetTask<IInMemoryQueueConsumer>();
            _cancellationToken.Token.Register(() => _consumer.TrySetCanceled());
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

            Interlocked.Increment(ref _queueDepth);

            Task.Factory.StartNew(() => DispatchMessage(context.Package), _cancellationToken.Token, TaskCreationOptions.None, _scheduler);

            return Task.FromResult(true);
        }

        async Task DispatchMessage(InMemoryTransportMessage message)
        {
            var consumer = await _consumer.Task.ConfigureAwait(false);

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

        public override string ToString()
        {
            return $"Queue({_name})";
        }
    }
}
