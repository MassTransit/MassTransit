namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Middleware;
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

            IDeadLetterTransport deadLetterTransport =
                new InMemoryMessageDeadLetterTransport(_context.MessageFabric.GetExchange(_context.TransportContext, $"{_queueName}_skipped"));
            _context.AddOrUpdatePayload(() => deadLetterTransport, _ => deadLetterTransport);

            IErrorTransport errorTransport =
                new InMemoryMessageErrorTransport(_context.MessageFabric.GetExchange(_context.TransportContext, $"{_queueName}_error"));
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
            Agent,
            ReceiveTransportHandle,
            IMessageReceiver<InMemoryTransportMessage>
        {
            readonly InMemoryReceiveEndpointContext _context;
            readonly IReceivePipeDispatcher _dispatcher;
            readonly ChannelExecutor _executor;
            readonly IMessageQueue<InMemoryTransportContext, InMemoryTransportMessage> _queue;
            TopologyHandle _topologyHandle;

            public ReceiveTransportAgent(InMemoryReceiveEndpointContext context, IMessageQueue<InMemoryTransportContext, InMemoryTransportMessage> queue)
            {
                _context = context;
                _queue = queue;

                _dispatcher = context.CreateReceivePipeDispatcher();

                _executor = new ChannelExecutor(context.ConcurrentMessageLimit ?? context.PrefetchCount, false);

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
                        await _dispatcher.Dispatch(context).ConfigureAwait(false);
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

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await this.Stop("Stop Receive Transport", cancellationToken).ConfigureAwait(false);
            }

            async Task Startup()
            {
                try
                {
                    await _context.Dependencies.OrCanceled(Stopping).ConfigureAwait(false);

                    _topologyHandle = _queue.ConnectMessageReceiver(_context.TransportContext, this);

                    await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

                    SetReady();
                }
                catch (Exception exception)
                {
                    SetNotReady(exception);
                }
            }

            protected override async Task StopAgent(StopContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                _topologyHandle?.Disconnect();

                await _executor.DisposeAsync().ConfigureAwait(false);

                var metrics = _dispatcher.GetMetrics();

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);

                await base.StopAgent(context).ConfigureAwait(false);
            }
        }
    }
}
