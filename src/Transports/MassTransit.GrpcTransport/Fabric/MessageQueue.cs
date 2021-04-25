namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Metrics;


    public class MessageQueue :
        Agent,
        IMessageQueue
    {
        readonly Channel<DeliveryContext<GrpcTransportMessage>> _channel;
        readonly Task _dispatcher;
        readonly QueueMetric _metrics;
        readonly string _name;
        readonly IMessageFabricObserver _observer;
        readonly MessageReceiverCollection _receivers;

        public MessageQueue(IMessageFabricObserver observer, string name)
        {
            _observer = observer;
            _name = name;

            _receivers = new MessageReceiverCollection(receivers => new RoundRobinReceiverLoadBalancer(receivers));
            _metrics = new QueueMetric(name);

            _channel = Channel.CreateUnbounded<DeliveryContext<GrpcTransportMessage>>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false
            });

            _dispatcher = Task.Run(() => StartDispatcher());
        }

        public TopologyHandle ConnectMessageReceiver(NodeContext nodeContext, IMessageReceiver receiver)
        {
            try
            {
                var handle = _receivers.Connect(receiver);

                handle = _observer.ConsumerConnected(nodeContext, handle, _name);

                return handle;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {_name}", exception);
            }
        }

        public async Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            if (context.WasAlreadyDelivered(this))
                return;

            if (context.Message.EnqueueTime.HasValue)
                DeliverWithDelay(context);
            else
            {
                await _channel.Writer.WriteAsync(context, context.CancellationToken).ConfigureAwait(false);

                _metrics.MessageCount.Add();
            }
        }

        public Task Send(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("queue");
            scope.Add("name", _name);

            _receivers.Probe(scope);
        }

        protected override async Task StopAgent(StopContext context)
        {
            _channel.Writer.Complete();

            await _channel.Reader.Completion.ConfigureAwait(false);

            await _dispatcher.ConfigureAwait(false);

            await base.StopAgent(context).ConfigureAwait(false);
        }

        void DeliverWithDelay(DeliveryContext<GrpcTransportMessage> context)
        {
            Task.Run(async () =>
            {
                var delayed = false;
                try
                {
                    var delay = context.Message.EnqueueTime.Value - DateTime.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        _metrics.DelayedMessageCount.Add();
                        delayed = true;

                        await Task.Delay(delay, Stopping).ConfigureAwait(false);
                    }

                    await _channel.Writer.WriteAsync(context, Stopping).ConfigureAwait(false);
                    _metrics.MessageCount.Add();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    LogContext.Error?.Log(exception, "Message delivery faulted: {Queue}", _name);
                }
                finally
                {
                    if (delayed)
                        _metrics.DelayedMessageCount.Remove();
                }
            }, context.CancellationToken);
        }

        async Task StartDispatcher()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(Stopping).ConfigureAwait(false))
                {
                    DeliveryContext<GrpcTransportMessage> context = await _channel.Reader.ReadAsync(Stopping).ConfigureAwait(false);
                    _metrics.MessageCount.Remove();

                    try
                    {
                        IMessageReceiver receiver = null;
                        if (context.Message.Message.Deliver.DestinationCase == Contracts.Deliver.DestinationOneofCase.Receiver)
                        {
                            var receiverDestination = context.Message.Message.Deliver.Receiver;
                            if (!_receivers.TryGetReceiver(receiverDestination.ReceiverId, out receiver))
                                LogContext.Debug?.Log("Received not found: {Queue}, {ReceiverId}", _name, receiverDestination.ReceiverId);
                        }

                        receiver ??= await _receivers.Next(context.Message, Stopping).ConfigureAwait(false);
                        if (receiver != null)
                            await receiver.Deliver(context.Message, Stopping).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Warning?.Log(exception, "Failed to dispatch message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Queue dispatcher faulted: {Queue}", _name);
            }
        }
    }
}
