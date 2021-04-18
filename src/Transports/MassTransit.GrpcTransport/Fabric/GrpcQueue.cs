namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Util;
    using Util;


    public class GrpcQueue :
        IGrpcQueue
    {
        readonly TaskCompletionSource<IGrpcQueueConsumer> _consumer;
        readonly Connectable<IGrpcQueueConsumer> _consumers;
        readonly ChannelExecutor _executor;
        readonly string _name;
        readonly IMessageFabricObserver _observer;

        public GrpcQueue(IMessageFabricObserver observer, string name, int concurrencyLevel)
        {
            _observer = observer;
            _name = name;

            _consumers = new Connectable<IGrpcQueueConsumer>();
            _consumer = Util.TaskUtil.GetTask<IGrpcQueueConsumer>();

            _executor = new ChannelExecutor(concurrencyLevel, false);
        }

        public ConnectHandle ConnectConsumer(NodeContext nodeContext, IGrpcQueueConsumer consumer)
        {
            try
            {
                var handle = _consumers.Connect(consumer);

                handle = _observer.ConsumerConnected(nodeContext, handle, _name);

                _consumer.TrySetResult(consumer);

                return handle;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {_name}", exception);
            }
        }

        public Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            return context.WasAlreadyDelivered(this)
                ? Task.CompletedTask
                : context.Message.EnqueueTime.HasValue
                    ? DeliverWithDelay(context)
                    : _executor.Push(() => DispatchMessage(context), context.CancellationToken);
        }

        public Task Send(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public ValueTask DisposeAsync()
        {
            return _executor.DisposeAsync();
        }

        async Task DeliverWithDelay(DeliveryContext<GrpcTransportMessage> context)
        {
            var delay = context.Message.EnqueueTime.Value - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, context.CancellationToken).ConfigureAwait(false);

            await _executor.Push(() => DispatchMessage(context), context.CancellationToken).ConfigureAwait(false);
        }

        async Task DispatchMessage(DeliveryContext<GrpcTransportMessage> context)
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
    }
}
