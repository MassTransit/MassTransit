namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.Azure.ServiceBus;
    using Pipeline;
    using Policies;
    using Transports;


    public class ReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly IPipe<ClientContext> _clientPipe;
        readonly ServiceBusReceiveEndpointContext _context;
        readonly ClientSettings _settings;

        public ReceiveTransport(ClientSettings settings, IClientContextSupervisor clientContextSupervisor, IPipe<ClientContext> clientPipe,
            ServiceBusReceiveEndpointContext context)
        {
            _settings = settings;
            _clientContextSupervisor = clientContextSupervisor;
            _clientPipe = clientPipe;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Set(new
            {
                Type = "Azure Service Bus",
                _settings.Path,
                _settings.PrefetchCount,
                _settings.MaxConcurrentCalls
            });
        }

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
                    await _context.RetryPolicy.Retry(async () =>
                    {
                        try
                        {
                            await _context.OnTransportStartup(_clientContextSupervisor, Stopping).ConfigureAwait(false);
                            if (IsStopping)
                                return;

                            await _clientContextSupervisor.Send(_clientPipe, Stopped).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException ex)
                        {
                            throw await ConvertToServiceBusConnectionException(ex, "Operation interrupted: ").ConfigureAwait(false);
                        }
                        catch (UnauthorizedException ex)
                        {
                            throw await ConvertToServiceBusConnectionException(ex, "Unauthorized: ").ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            throw await ConvertToServiceBusConnectionException(ex, "ReceiveTransport Faulted, Restarting: ").ConfigureAwait(false);
                        }
                    }, Stopping).ConfigureAwait(false);
                }
                catch
                {
                    // i said, nothing to see here
                }
            }
        }

        async Task<ServiceBusConnectionException> ConvertToServiceBusConnectionException(Exception ex, string message)
        {
            var exception = new ServiceBusConnectionException(message + _context.InputAddress, ex);

            await NotifyFaulted(exception).ConfigureAwait(false);

            return exception;
        }

        Task NotifyFaulted(ServiceBusConnectionException exception)
        {
            LogContext.Error?.Log(exception, "Receive Transport Faulted: {InputAddress}", _context.InputAddress);

            return _context.TransportObservers.Faulted(new ReceiveTransportFaultedEvent(_context.InputAddress, exception));
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
