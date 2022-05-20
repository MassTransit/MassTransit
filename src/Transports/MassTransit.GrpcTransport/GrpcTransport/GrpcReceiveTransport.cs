namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Fabric;
    using Internals;
    using MassTransit.Middleware;
    using Transports;
    using Transports.Fabric;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class GrpcReceiveTransport :
        IReceiveTransport
    {
        readonly GrpcReceiveEndpointContext _context;
        readonly string _queueName;

        public GrpcReceiveTransport(GrpcReceiveEndpointContext context, string queueName)
        {
            _context = context;
            _queueName = queueName;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiveTransport");
            scope.Add("type", "gRPC");
            scope.Set(new
            {
                Address = _context.InputAddress,
                _context.PrefetchCount,
                _context.ConcurrentMessageLimit
            });
        }

        ReceiveTransportHandle IReceiveTransport.Start()
        {
            return new ReceiveTransportAgent(_context, _queueName);
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
            IMessageReceiver<GrpcTransportMessage>
        {
            readonly Channel<GrpcTransportMessage> _channel;
            readonly Task _consumeDispatcher;
            readonly GrpcReceiveEndpointContext _context;
            readonly IReceivePipeDispatcher _dispatcher;
            readonly IConcurrencyLimiter _limiter;
            readonly string _queueName;
            TopologyHandle _topologyHandle;

            public ReceiveTransportAgent(GrpcReceiveEndpointContext context, string queueName)
            {
                _context = context;
                _queueName = queueName;

                _dispatcher = context.CreateReceivePipeDispatcher();

                var outputOptions = new BoundedChannelOptions(context.PrefetchCount)
                {
                    SingleWriter = true,
                    SingleReader = true,
                    AllowSynchronousContinuations = true
                };

                _channel = Channel.CreateBounded<GrpcTransportMessage>(outputOptions);

                if (context.ConcurrentMessageLimit.HasValue)
                    _limiter = new ConcurrencyLimiter(context.ConcurrentMessageLimit.Value);

                _consumeDispatcher = Task.Run(() => StartDispatcher());

                var startup = Task.Run(() => Startup());

                SetReady(startup);
            }

            public async Task Deliver(GrpcTransportMessage message, CancellationToken cancellationToken)
            {
                if (IsStopped)
                    return;

                await _channel.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
            }

            public void Probe(ProbeContext context)
            {
                var scope = context.CreateScope("local");
                scope.Add("nodeAddress", _context.TransportProvider.HostNodeContext.NodeAddress);
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await this.Stop("Stop Receive Transport", cancellationToken).ConfigureAwait(false);
            }

            async Task StartDispatcher()
            {
                LogContext.Current = _context.LogContext;

                try
                {
                    await Ready.ConfigureAwait(false);

                    while (await _channel.Reader.WaitToReadAsync(Stopping).ConfigureAwait(false))
                    {
                        var message = await _channel.Reader.ReadAsync(Stopping).ConfigureAwait(false);

                        if (_limiter != null)
                            await _limiter.Wait(Stopping).ConfigureAwait(false);

                        _ = Task.Run(async () =>
                        {
                            var context = new GrpcReceiveContext(message, _context);
                            try
                            {
                                await _dispatcher.Dispatch(context).ConfigureAwait(false);
                            }
                            catch (Exception exception)
                            {
                                context.LogTransportFaulted(exception);
                            }
                            finally
                            {
                                _limiter?.Release();
                                context.Dispose();
                            }
                        }, Stopping);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    LogContext.Warning?.Log(exception, "Consumer dispatcher faulted: {Queue}", _queueName);
                }
            }

            async Task Startup()
            {
                try
                {
                    await _context.TransportProvider.StartupTask.OrCanceled(Stopping).ConfigureAwait(false);

                    await _context.Dependencies.OrCanceled(Stopping).ConfigureAwait(false);

                    var hostNodeContext = _context.TransportProvider.HostNodeContext;

                    var queue = _context.MessageFabric.GetQueue(hostNodeContext, _queueName);

                    IDeadLetterTransport deadLetterTransport =
                        new GrpcDeadLetterTransport(_context.MessageFabric.GetExchange(hostNodeContext, $"{_queueName}_skipped"));
                    _context.AddOrUpdatePayload(() => deadLetterTransport, _ => deadLetterTransport);

                    IErrorTransport errorTransport =
                        new GrpcErrorTransport(_context.MessageFabric.GetExchange(hostNodeContext, $"{_queueName}_error"));
                    _context.AddOrUpdatePayload(() => errorTransport, _ => errorTransport);

                    _context.ConfigureTopology(hostNodeContext);

                    _topologyHandle = queue.ConnectMessageReceiver(hostNodeContext, this);

                    await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    SetNotReady(exception);
                    throw;
                }
            }

            protected override async Task StopAgent(StopContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                _channel.Writer.Complete();

                await _channel.Reader.Completion.ConfigureAwait(false);

                await _consumeDispatcher.ConfigureAwait(false);

                var metrics = _dispatcher.GetMetrics();

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);

                _topologyHandle?.Disconnect();

                await base.StopAgent(context).ConfigureAwait(false);
            }
        }
    }
}
