namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using ConsumePipeSpecifications;
    using Context;
    using EndpointConfigurators;
    using Events;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Pipeline;
    using Topology;
    using Transports;
    using Util;


    public class MassTransitBus :
        IBusControl
    {
        readonly IBusObserver _busObservable;
        readonly IConsumePipe _consumePipe;
        readonly IConsumePipeSpecification _consumePipeSpecification;
        readonly IBusHostControl _host;
        readonly ILogContext _logContext;
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly IReceiveEndpoint _receiveEndpoint;
        Handle _busHandle;

        public MassTransitBus(IBusHostControl host, IBusObserver busObservable, IReceiveEndpointConfiguration endpointConfiguration)
        {
            Address = endpointConfiguration.InputAddress;
            _consumePipe = endpointConfiguration.ConsumePipe;
            _consumePipeSpecification = endpointConfiguration.Consume.Specification;
            _host = host;
            _busObservable = busObservable;
            _receiveEndpoint = endpointConfiguration.ReceiveEndpoint;

            Topology = host.Topology;

            _logContext = LogContext.Current;

            _publishEndpoint = new Lazy<IPublishEndpoint>(() => _receiveEndpoint.CreatePublishEndpoint(Address));
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            IPipe<ConsumeContext<T>> messagePipe = _consumePipeSpecification.GetMessageSpecification<T>().BuildMessagePipe(pipe);

            var handle = _consumePipe.ConnectConsumePipe(messagePipe);

            if (_busHandle != null && !_receiveEndpoint.Started.IsCompletedSuccessfully())
                TaskUtil.Await(_receiveEndpoint.Started);

            return handle;
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            IPipe<ConsumeContext<T>> messagePipe = _consumePipeSpecification.GetMessageSpecification<T>().BuildMessagePipe(pipe);

            var handle = _consumePipe.ConnectRequestPipe(requestId, messagePipe);

            if (_busHandle != null && !_receiveEndpoint.Started.IsCompletedSuccessfully())
                TaskUtil.Await(_receiveEndpoint.Started);

            return handle;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Value.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Uri Address { get; }

        public IBusTopology Topology { get; }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _receiveEndpoint.GetSendEndpoint(address);
        }

        public async Task<BusHandle> StartAsync(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            if (_busHandle != null)
            {
                LogContext.Warning?.Log("StartAsync called, but the bus was already started: {Address} ({Reason})", Address, "Already Started");
                return _busHandle;
            }

            await _busObservable.PreStart(this).ConfigureAwait(false);

            Handle busHandle = null;

            CancellationTokenSource tokenSource = null;
            try
            {
                if (cancellationToken == default)
                {
                    tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    cancellationToken = tokenSource.Token;
                }

                var hostHandle = await _host.Start(cancellationToken).ConfigureAwait(false);

                busHandle = new Handle(hostHandle, this, _busObservable, _logContext);

                await busHandle.Ready.ConfigureAwait(false);

                await _busObservable.PostStart(this, busHandle.Ready).ConfigureAwait(false);

                _busHandle = busHandle;

                return _busHandle;
            }
            catch (Exception ex)
            {
                try
                {
                    if (busHandle != null)
                    {
                        LogContext.Debug?.Log("Bus start faulted, stopping host");

                        await busHandle.StopAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception stopException)
                {
                    LogContext.Warning?.Log(stopException, "Bus start faulted, and failed to stop host");
                }

                await _busObservable.StartFaulted(this, ex).ConfigureAwait(false);

                throw;
            }
            finally
            {
                tokenSource?.Dispose();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            LogContext.SetCurrentIfNull(_logContext);

            if (_busHandle == null)
            {
                LogContext.Warning?.Log("Failed to stop bus: {Address} ({Reason})", Address, "Not Started");
                return TaskUtil.Completed;
            }

            return _busHandle.StopAsync(cancellationToken);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _host.ConnectConsumeObserver(observer);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _host.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _host.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _host.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _host.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _host.ConnectSendObserver(observer);
        }

        ConnectHandle IEndpointConfigurationObserverConnector.ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _host.ConnectEndpointConfigurationObserver(observer);
        }

        HostReceiveEndpointHandle IReceiveConnector.ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        HostReceiveEndpointHandle IReceiveConnector.ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            return _host.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("bus");
            scope.Add("address", Address);

            _host.Probe(scope);
        }


        class Handle :
            BusHandle
        {
            readonly IBus _bus;
            readonly IBusObserver _busObserver;
            readonly ILogContext _logContext;
            readonly HostHandle _hostHandle;
            bool _stopped;

            public Handle(HostHandle hostHandle, IBus bus, IBusObserver busObserver, ILogContext logContext)
            {
                _bus = bus;
                _busObserver = busObserver;
                _logContext = logContext;
                _hostHandle = hostHandle;
            }

            public Task<BusReady> Ready => ReadyOrNot(_hostHandle.Ready);

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                LogContext.SetCurrentIfNull(_logContext);

                if (_stopped)
                    return;

                await _busObserver.PreStop(_bus).ConfigureAwait(false);

                try
                {
                    LogContext.Debug?.Log("Stopping host");

                    await _hostHandle.Stop(cancellationToken).ConfigureAwait(false);

                    await _busObserver.PostStop(_bus).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    await _busObserver.StopFaulted(_bus, exception).ConfigureAwait(false);

                    LogContext.Warning?.Log(exception, "Bus stop faulted");

                    throw;
                }

                _stopped = true;
            }

            async Task<BusReady> ReadyOrNot(Task<HostReady> ready)
            {
                var hostReady = await ready.ConfigureAwait(false);

                return new BusReadyEvent(hostReady, _bus);
            }
        }
    }
}
