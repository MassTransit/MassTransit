namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Transports;


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
            IGrpcQueueConsumer
        {
            readonly GrpcReceiveEndpointContext _context;
            readonly IReceivePipeDispatcher _dispatcher;
            readonly string _queueName;
            ConnectHandle _consumerHandle;

            public ReceiveTransportAgent(GrpcReceiveEndpointContext context, string queueName)
            {
                _context = context;
                _queueName = queueName;

                _dispatcher = context.CreateReceivePipeDispatcher();

                var startup = Task.Run(() => Startup());

                SetReady(startup);
            }

            public async Task Consume(GrpcTransportMessage message, CancellationToken cancellationToken)
            {
                await Ready.ConfigureAwait(false);

                if (IsStopped)
                    return;

                LogContext.Current = _context.LogContext;

                await using var context = new GrpcReceiveContext(message, _context, cancellationToken);
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
                    context.Dispose();
                }
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await this.Stop("Stop Receive Transport", cancellationToken).ConfigureAwait(false);
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
                        new GrpcMessageDeadLetterTransport(_context.MessageFabric.GetExchange(hostNodeContext, $"{_queueName}_skipped"));
                    _context.AddOrUpdatePayload(() => deadLetterTransport, _ => deadLetterTransport);

                    IErrorTransport errorTransport =
                        new GrpcMessageErrorTransport(_context.MessageFabric.GetExchange(hostNodeContext, $"{_queueName}_error"));
                    _context.AddOrUpdatePayload(() => errorTransport, _ => errorTransport);

                    _context.ConfigureTopology(hostNodeContext);

                    _consumerHandle = queue.ConnectConsumer(hostNodeContext, this);

                    await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(_context.InputAddress));
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

                var metrics = _dispatcher.GetMetrics();
                var completed = new ReceiveTransportCompletedEvent(_context.InputAddress, metrics);

                await _context.TransportObservers.Completed(completed).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);

                _consumerHandle?.Disconnect();

                await base.StopAgent(context).ConfigureAwait(false);
            }
        }
    }
}
