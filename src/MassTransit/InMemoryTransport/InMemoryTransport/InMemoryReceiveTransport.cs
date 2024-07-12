namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Internals;
    using Transports;
    using Transports.Fabric;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemoryReceiveTransport :
        IReceiveTransport
    {
        readonly InMemoryReceiveEndpointContext _context;
        readonly string _queueName;

        public InMemoryReceiveTransport(InMemoryReceiveEndpointContext context, string queueName)
        {
            _context = context;
            _queueName = queueName;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiveTransport");
            scope.Add("type", "InMemory");
            scope.Set(new
            {
                Address = _context.InputAddress,
                _context.PrefetchCount,
                _context.ConcurrentMessageLimit
            });
        }

        ReceiveTransportHandle IReceiveTransport.Start()
        {
            _context.ConfigureTopology();

            IMessageQueue<InMemoryTransportContext, InMemoryTransportMessage> queue =
                _context.MessageFabric.GetQueue(_context.TransportContext, _queueName);

            IDeadLetterTransport deadLetterTransport = new InMemoryMessageDeadLetterTransport(_context.MessageFabric.GetExchange(_context.TransportContext,
                _context.Send.DeadLetterQueueNameFormatter.FormatDeadLetterQueueName(_queueName)));
            _context.AddOrUpdatePayload(() => deadLetterTransport, _ => deadLetterTransport);

            IErrorTransport errorTransport = new InMemoryMessageErrorTransport(_context.MessageFabric.GetExchange(_context.TransportContext,
                _context.Send.ErrorQueueNameFormatter.FormatErrorQueueName(_queueName)));
            _context.AddOrUpdatePayload(() => errorTransport, _ => errorTransport);

            return new ReceiveTransportAgent(_context, queue);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _context.ConnectReceiveTransportObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }


        class ReceiveTransportAgent :
            ConsumerAgent<long>,
            ReceiveTransportHandle,
            IMessageReceiver<InMemoryTransportMessage>
        {
            readonly InMemoryReceiveEndpointContext _context;
            readonly TaskExecutor _executor;
            readonly IMessageQueue<InMemoryTransportContext, InMemoryTransportMessage> _queue;
            TopologyHandle _topologyHandle;

            public ReceiveTransportAgent(InMemoryReceiveEndpointContext context, IMessageQueue<InMemoryTransportContext, InMemoryTransportMessage> queue)
                : base(context)
            {
                _context = context;
                _queue = queue;

                _executor = new TaskExecutor(context.ConcurrentMessageLimit ?? context.PrefetchCount);

                Task.Run(() => Startup());
            }

            public Task Deliver(InMemoryTransportMessage message, CancellationToken cancellationToken)
            {
                if (IsStopping)
                    return Task.CompletedTask;

                return _executor.Push(async () =>
                {
                    LogContext.Current = _context.LogContext;

                    var context = new InMemoryReceiveContext(message, _context);

                    try
                    {
                        await Dispatch(message.SequenceNumber, context, NoLockReceiveContext.Instance).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        message.DeliveryCount++;
                        context.LogTransportFaulted(exception);
                    }
                    finally
                    {
                        context.Dispose();
                    }
                }, cancellationToken);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("inMemory");
            }

            Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                return this.Stop("Stop Receive Transport", cancellationToken);
            }

            async Task Startup()
            {
                try
                {
                    await _context.DependenciesReady.OrCanceled(Stopping).ConfigureAwait(false);

                    _topologyHandle = _queue.ConnectMessageReceiver(_context.TransportContext, this);

                    await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

                    SetReady();
                }
                catch (Exception exception)
                {
                    SetNotReady(exception);
                }
            }

            protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
            {
                _topologyHandle?.Disconnect();

                await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

                await _executor.DisposeAsync().ConfigureAwait(false);

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, this).ConfigureAwait(false);

                _context.LogConsumerCompleted(DeliveryCount, ConcurrentDeliveryCount);
            }
        }
    }
}
