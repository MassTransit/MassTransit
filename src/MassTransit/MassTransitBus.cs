namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Events;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transports;
    using Util;


    public class MassTransitBus :
        IBusControl
    {
        readonly IBusObserver _busObservable;
        readonly IConsumePipe _consumePipe;
        readonly IReadOnlyHostCollection _hosts;
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly ISendEndpointProvider _sendEndpointProvider;
        Handle _busHandle;

        public MassTransitBus(Uri address, IConsumePipe consumePipe, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider, IReadOnlyHostCollection hosts, IBusObserver busObservable)
        {
            Address = address;
            _consumePipe = consumePipe;
            _sendEndpointProvider = sendEndpointProvider;
            _busObservable = busObservable;
            _hosts = hosts;

            Topology = hosts.GetBusTopology();

            _publishEndpoint = new Lazy<IPublishEndpoint>(() => publishEndpointProvider.CreatePublishEndpoint(address));
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _consumePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _consumePipe.ConnectRequestPipe(requestId, pipe);
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
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        public async Task<BusHandle> StartAsync(CancellationToken cancellationToken)
        {
            if (_busHandle != null)
            {
                LogContext.Warning?.Log("StartAsync called, but the bus was already started: {Address} ({Reason})", Address, "Already Started");
                return _busHandle;
            }

            await _busObservable.PreStart(this).ConfigureAwait(false);

            Handle busHandle = null;

            CancellationTokenSource tokenSource = null;
            var hosts = new List<HostHandle>();
            try
            {
                if (cancellationToken == default)
                {
                    tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    cancellationToken = tokenSource.Token;
                }

                foreach (var host in _hosts)
                {
                    var hostHandle = await host.Start(cancellationToken).ConfigureAwait(false);

                    hosts.Add(hostHandle);
                }

                busHandle = new Handle(hosts, this, _busObservable);

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
                        LogContext.Debug?.Log("Bus start faulted, stopping hosts");

                        await busHandle.StopAsync(cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        var handle = new Handle(hosts, this, _busObservable);

                        await handle.StopAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception stopException)
                {
                    LogContext.Warning?.Log(stopException, "Bus start faulted, and failed to stop started hosts");
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
            if (_busHandle == null)
            {
                LogContext.Warning?.Log("Failed to stop bus: {Address} ({Reason})", Address, "Not Started");
                return TaskUtil.Completed;
            }

            return _busHandle.StopAsync(cancellationToken);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return new MultipleConnectHandle(_hosts.Select(h => h.ConnectConsumeObserver(observer)));
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return new MultipleConnectHandle(_hosts.Select(h => h.ConnectConsumeMessageObserver(observer)));
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return new MultipleConnectHandle(_hosts.Select(x => x.ConnectReceiveObserver(observer)));
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return new MultipleConnectHandle(_hosts.Select(x => x.ConnectReceiveEndpointObserver(observer)));
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return new MultipleConnectHandle(_hosts.Select(h => h.ConnectPublishObserver(observer)));
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return new MultipleConnectHandle(_hosts.Select(h => h.ConnectSendObserver(observer)));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("bus");
            scope.Add("address", Address);

            foreach (var host in _hosts)
                host.Probe(scope);
        }


        class Handle :
            BusHandle
        {
            readonly IBus _bus;
            readonly IBusObserver _busObserver;
            readonly HostHandle[] _hostHandles;
            bool _stopped;

            public Handle(IEnumerable<HostHandle> hostHandles, IBus bus, IBusObserver busObserver)
            {
                _bus = bus;
                _busObserver = busObserver;
                _hostHandles = hostHandles.ToArray();
            }

            public Task<BusReady> Ready => ReadyOrNot(_hostHandles.Select(x => x.Ready));

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                await _busObserver.PreStop(_bus).ConfigureAwait(false);

                try
                {
                    LogContext.Debug?.Log("Stopping hosts");

                    await Task.WhenAll(_hostHandles.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);

                    await _busObserver.PostStop(_bus).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await _busObserver.StopFaulted(_bus, exception).ConfigureAwait(false);

                    LogContext.Warning?.Log(exception, "Bus stop faulted");

                    throw;
                }

                _stopped = true;
            }

            async Task<BusReady> ReadyOrNot(IEnumerable<Task<HostReady>> hosts)
            {
                Task<HostReady>[] readyTasks = hosts as Task<HostReady>[] ?? hosts.ToArray();
                foreach (Task<HostReady> ready in readyTasks)
                {
                    await ready.ConfigureAwait(false);
                }

                HostReady[] hostsReady = await Task.WhenAll(readyTasks).ConfigureAwait(false);

                return new BusReadyEvent(hostsReady, _bus);
            }
        }
    }
}
