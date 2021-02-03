namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
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
        readonly IHost _host;
        readonly ILogContext _logContext;
        readonly IPublishEndpoint _publishEndpoint;
        readonly IReceiveEndpoint _receiveEndpoint;
        Handle _busHandle;

        public MassTransitBus(IHost host, IBusObserver busObservable, IReceiveEndpointConfiguration endpointConfiguration)
        {
            Address = endpointConfiguration.InputAddress;
            _consumePipe = endpointConfiguration.ConsumePipe;
            _host = host;
            _busObservable = busObservable;
            _receiveEndpoint = endpointConfiguration.ReceiveEndpoint;

            Topology = host.Topology;

            if (LogContext.Current == null)
                throw new ConfigurationException("The LogContext was not set.");

            _logContext = LogContext.Current;

            _publishEndpoint = new PublishEndpoint(_receiveEndpoint);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            LogContext.SetCurrentIfNull(_logContext);

            var handle = _consumePipe.ConnectConsumePipe(pipe);

            if (_busHandle != null && !_receiveEndpoint.Started.IsCompletedSuccessfully())
                TaskUtil.Await(_receiveEndpoint.Started);

            return handle;
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            LogContext.SetCurrentIfNull(_logContext);

            var handle = _consumePipe.ConnectConsumePipe(pipe, options);

            if (_busHandle != null && !_receiveEndpoint.Started.IsCompletedSuccessfully())
                TaskUtil.Await(_receiveEndpoint.Started);

            return handle;
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            LogContext.SetCurrentIfNull(_logContext);

            var handle = _consumePipe.ConnectRequestPipe(requestId, pipe);

            if (_busHandle != null && !_receiveEndpoint.Started.IsCompletedSuccessfully())
                TaskUtil.Await(_receiveEndpoint.Started);

            return handle;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
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

                var hostHandle = _host.Start(cancellationToken);

                busHandle = new Handle(hostHandle, this, _busObservable, _logContext);

                try
                {
                    await busHandle.Ready.OrCanceled(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == cancellationToken)
                {
                    try
                    {
                        await busHandle.StopAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        LogContext.Warning?.Log(ex, "Bus start faulted, and failed to stop host");
                    }

                    await busHandle.Ready.ConfigureAwait(false);
                }

                await _busObservable.PostStart(this, busHandle.Ready).ConfigureAwait(false);

                _busHandle = busHandle;

                LogContext.Debug?.Log("Bus started: {HostAddress}", _host.Address);

                return _busHandle;
            }
            catch (Exception ex)
            {
                try
                {
                    if (busHandle != null)
                    {
                        LogContext.Debug?.Log(ex, "Bus start faulted, stopping host");

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

        public async Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            LogContext.SetCurrentIfNull(_logContext);

            if (_busHandle == null)
            {
                LogContext.Warning?.Log("Failed to stop bus: {Address} ({Reason})", Address, "Not Started");
                return;
            }

            await _busHandle.StopAsync(cancellationToken).ConfigureAwait(false);

            _busHandle = null;
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _host.ConnectConsumeObserver(observer);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _host.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _host.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _host.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _host.ConnectPublishObserver(observer);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _receiveEndpoint.GetPublishSendEndpoint<T>();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _host.ConnectSendObserver(observer);
        }

        ConnectHandle IEndpointConfigurationObserverConnector.ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            LogContext.SetCurrentIfNull(_logContext);

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
            readonly HostHandle _hostHandle;
            readonly ILogContext _logContext;
            bool _stopped;

            public Handle(HostHandle hostHandle, IBus bus, IBusObserver busObserver, ILogContext logContext)
            {
                _bus = bus;
                _busObserver = busObserver;
                _logContext = logContext;
                _hostHandle = hostHandle;

                Ready = ReadyOrNot(hostHandle.Ready);
            }

            public Task<BusReady> Ready { get; }

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                LogContext.SetCurrentIfNull(_logContext);

                if (_stopped)
                    return;

                await _busObserver.PreStop(_bus).ConfigureAwait(false);

                try
                {
                    var hostAddress = _hostHandle.Ready.IsCompletedSuccessfully()
                        ? _hostHandle.Ready.GetAwaiter().GetResult().HostAddress
                        : new Uri(_bus.Address.GetLeftPart(UriPartial.Authority));

                    LogContext.Debug?.Log("Stopping host: {HostAddress}", hostAddress);

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
