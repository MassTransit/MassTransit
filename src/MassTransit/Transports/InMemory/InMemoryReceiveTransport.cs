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
    using Logging;
    using Metrics;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemoryReceiveTransport :
        Agent,
        IReceiveTransport,
        IInMemoryQueueConsumer
    {
        readonly Uri _inputAddress;
        readonly IInMemoryQueue _queue;
        readonly ReceiveEndpointContext _receiveEndpointContext;
        readonly IDeliveryTracker _tracker;

        public InMemoryReceiveTransport(Uri inputAddress, IInMemoryQueue queue, ReceiveEndpointContext receiveEndpointContext)
        {
            _inputAddress = inputAddress;
            _queue = queue;
            _receiveEndpointContext = receiveEndpointContext;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);
        }

        public async Task Consume(InMemoryTransportMessage message, CancellationToken cancellationToken)
        {
            await Ready.ConfigureAwait(false);
            if (IsStopped)
                return;

            LogContext.Current = _receiveEndpointContext.LogContext;

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveEndpointContext);
            var delivery = _tracker.BeginDelivery();

            var activity = LogContext.IfEnabled(OperationName.Transport.Receive)?.StartActivity();
            activity.AddReceiveContextHeaders(context);

            try
            {
                if (_receiveEndpointContext.ReceiveObservers.Count > 0)
                    await _receiveEndpointContext.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                await _receiveEndpointContext.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                if (_receiveEndpointContext.ReceiveObservers.Count > 0)
                    await _receiveEndpointContext.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_receiveEndpointContext.ReceiveObservers.Count > 0)
                    await _receiveEndpointContext.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                message.DeliveryCount++;
            }
            finally
            {
                activity?.Stop();

                delivery.Dispose();

                context.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("inMemoryReceiveTransport");
            scope.Set(new {Address = _inputAddress});
        }

        ReceiveTransportHandle IReceiveTransport.Start()
        {
            try
            {
                var consumerHandle = _queue.ConnectConsumer(this);

                void NotifyReady()
                {
                    _receiveEndpointContext.TransportObservers.Ready(new ReceiveTransportReadyEvent(_inputAddress));

                    SetReady();
                }

                Task.Factory.StartNew(NotifyReady, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

                return new Handle(this, consumerHandle);
            }
            catch (Exception exception)
            {
                SetNotReady(exception);
                throw;
            }
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveTransportObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }

        void HandleDeliveryComplete()
        {
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly InMemoryReceiveTransport _transport;
            readonly ConnectHandle _consumerHandle;

            public Handle(InMemoryReceiveTransport transport, ConnectHandle consumerHandle)
            {
                _transport = transport;
                _consumerHandle = consumerHandle;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                LogContext.SetCurrentIfNull(_transport._receiveEndpointContext.LogContext);

                await _transport.Stop("Stop", cancellationToken).ConfigureAwait(false);

                _consumerHandle.Disconnect();

                var completed = new ReceiveTransportCompletedEvent(_transport._inputAddress, _transport._tracker.GetDeliveryMetrics());

                await _transport._receiveEndpointContext.TransportObservers.Completed(completed).ConfigureAwait(false);
            }
        }
    }
}
