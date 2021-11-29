namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fabric;
    using Middleware;
    using Transports;


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
            var queue = _context.MessageFabric.GetQueue(_queueName);

            IDeadLetterTransport deadLetterTransport = new InMemoryMessageDeadLetterTransport(_context.MessageFabric.GetExchange($"{_queueName}_skipped"));
            _context.AddOrUpdatePayload(() => deadLetterTransport, _ => deadLetterTransport);

            IErrorTransport errorTransport = new InMemoryMessageErrorTransport(_context.MessageFabric.GetExchange($"{_queueName}_error"));
            _context.AddOrUpdatePayload(() => errorTransport, _ => errorTransport);

            _context.ConfigureTopology();

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
            IInMemoryQueueConsumer
        {
            readonly ConnectHandle _consumerHandle;
            readonly InMemoryReceiveEndpointContext _context;
            readonly IReceivePipeDispatcher _dispatcher;
            readonly IInMemoryQueue _queue;

            public ReceiveTransportAgent(InMemoryReceiveEndpointContext context, IInMemoryQueue queue)
            {
                _context = context;
                _queue = queue;

                _dispatcher = context.CreateReceivePipeDispatcher();

                _consumerHandle = queue.ConnectConsumer(this);

                Task.Run(() => Startup());
            }

            public async Task Consume(InMemoryTransportMessage message, CancellationToken cancellationToken)
            {
                await Ready.ConfigureAwait(false);
                if (IsStopped)
                    return;

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
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await this.Stop("Stop Receive Transport", cancellationToken).ConfigureAwait(false);
            }

            async Task Startup()
            {
                try
                {
                    await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

                    SetReady();
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

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);

                _consumerHandle.Disconnect();

                await base.StopAgent(context).ConfigureAwait(false);
            }
        }
    }
}
