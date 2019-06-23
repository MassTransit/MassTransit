namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Policies;
    using RabbitMQ.Client.Exceptions;
    using Topology;
    using Transports;


    public class RabbitMqReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        readonly IPipe<ConnectionContext> _connectionPipe;
        readonly IRabbitMqHost _host;
        readonly Uri _inputAddress;
        readonly ReceiveSettings _settings;
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;

        public RabbitMqReceiveTransport(IRabbitMqHost host, ReceiveSettings settings, IPipe<ConnectionContext> connectionPipe,
            RabbitMqReceiveEndpointContext receiveEndpointContext)
        {
            _host = host;
            _settings = settings;
            _receiveEndpointContext = receiveEndpointContext;
            _connectionPipe = connectionPipe;

            _inputAddress = receiveEndpointContext.InputAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "RabbitMQ");
            scope.Set(_settings);
            var topologyScope = scope.CreateScope("topology");
            _receiveEndpointContext.BrokerTopology.Probe(topologyScope);
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
                            await _host.ConnectionContextSupervisor.Send(_connectionPipe, Stopped).ConfigureAwait(false);
                        }
                        catch (RabbitMqConnectionException ex)
                        {
                            await NotifyFaulted(ex).ConfigureAwait(false);
                            throw;
                        }
                        catch (BrokerUnreachableException ex)
                        {
                            throw await ConvertToRabbitMqConnectionException(ex, "RabbitMQ Unreachable").ConfigureAwait(false);
                        }
                        catch (OperationInterruptedException ex)
                        {
                            throw await ConvertToRabbitMqConnectionException(ex, "Operation interrupted").ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                            throw await ConvertToRabbitMqConnectionException(ex, "ReceiveTransport Faulted, Restarting").ConfigureAwait(false);
                        }
                    }, Stopping).ConfigureAwait(false);
                }
                catch
                {
                    // nothing to see here
                }
            }
        }

        async Task<RabbitMqConnectionException> ConvertToRabbitMqConnectionException(Exception ex, string message)
        {
            LogContext.Error?.Log(ex, message);

            var exception = new RabbitMqConnectionException(message + _host.ConnectionContextSupervisor, ex);

            await NotifyFaulted(exception).ConfigureAwait(false);

            return exception;
        }

        Task NotifyFaulted(RabbitMqConnectionException exception)
        {
            LogContext.Error?.Log(exception, "RabbitMQ Connect Failed: {Host}", _host.Settings.ToDescription());

            return _receiveEndpointContext.TransportObservers.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, exception));
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
