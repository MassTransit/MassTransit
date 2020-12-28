namespace MassTransit.Transports.InMemory
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
            var scope = context.CreateScope("inMemoryReceiveTransport");
            scope.Set(new {Address = _context.InputAddress});
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
            readonly InMemoryReceiveEndpointContext _context;
            readonly IReceivePipeDispatcher _dispatcher;
            readonly IInMemoryQueue _queue;
            ConnectHandle _consumerHandle;

            public ReceiveTransportAgent(InMemoryReceiveEndpointContext context, IInMemoryQueue queue)
            {
                _context = context;
                _queue = queue;

                _dispatcher = context.CreateReceivePipeDispatcher();


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

            Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                return this.Stop("Stop Receive Transport", cancellationToken);
            }

            async Task Startup()
            {
                try
                {
                    _consumerHandle = _queue.ConnectConsumer(this);

                    await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(_context.InputAddress));

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

                _consumerHandle.Disconnect();

                var metrics = _dispatcher.GetMetrics();
                var completed = new ReceiveTransportCompletedEvent(_context.InputAddress, metrics);

                await _context.TransportObservers.Completed(completed).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);

                await base.StopAgent(context).ConfigureAwait(false);
            }
        }
    }
}
