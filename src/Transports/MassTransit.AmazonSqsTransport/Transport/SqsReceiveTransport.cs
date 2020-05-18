namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using Policies;
    using Topology;
    using Transports;


    public class SqsReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        readonly IPipe<ConnectionContext> _connectionPipe;
        readonly IAmazonSqsHost _host;
        readonly Uri _inputAddress;
        readonly ReceiveSettings _settings;
        readonly SqsReceiveEndpointContext _context;

        public SqsReceiveTransport(IAmazonSqsHost host, ReceiveSettings settings, IPipe<ConnectionContext> connectionPipe,
            SqsReceiveEndpointContext context)
        {
            _host = host;
            _settings = settings;
            _context = context;
            _connectionPipe = connectionPipe;

            _inputAddress = context.InputAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "AmazonSQS");
            scope.Set(_settings);
            var topologyScope = scope.CreateScope("topology");
            _context.BrokerTopology.Probe(topologyScope);
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public ReceiveTransportHandle Start()
        {
            Task.Factory.StartNew(Receiver, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            return new Handle(this);
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

        async Task Receiver()
        {
            while (!IsStopping)
            {
                try
                {
                    await _host.ConnectionRetryPolicy.Retry(async () =>
                    {
                        if (IsStopping)
                            return;

                        try
                        {
                            await _context.OnTransportStartup(_host.ConnectionContextSupervisor, Stopping).ConfigureAwait(false);
                            if (IsStopping)
                                return;

                            await _host.ConnectionContextSupervisor.Send(_connectionPipe, Stopped).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                            throw await ConvertToAmazonSqsConnectionException(ex, "ReceiveTransport Faulted, Restarting ").ConfigureAwait(false);
                        }
                    }, Stopping).ConfigureAwait(false);
                }
                catch
                {
                    // i said, nothing to see here
                }
            }
        }

        async Task<AmazonSqsConnectException> ConvertToAmazonSqsConnectionException(Exception ex, string message)
        {
            LogContext.Error?.Log(ex, message);

            var exception = new AmazonSqsConnectException(message + _host.ConnectionContextSupervisor, ex);

            await NotifyFaulted(exception).ConfigureAwait(false);

            return exception;
        }

        Task NotifyFaulted(Exception exception)
        {
            LogContext.Error?.Log(exception, "AmazonSQS Connect Failed: {Host}", _host.Address);

            return _context.TransportObservers.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, exception));
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly IAgent _agent;

            public Handle(IAgent agent)
            {
                _agent = agent;
            }

            Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                return _agent.Stop("Stop Receive Transport", cancellationToken);
            }
        }
    }
}
