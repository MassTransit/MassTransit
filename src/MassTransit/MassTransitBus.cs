// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Internals.Extensions;
    using Logging;
    using Pipeline;
    using Transports;
    using Util;


    public class MassTransitBus :
        IBusControl,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<MassTransitBus>();
        readonly IBusObserver _busObservable;
        readonly IConsumePipe _consumePipe;
        readonly IBusHostControl[] _hosts;
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly IReceiveEndpoint[] _receiveEndpoints;
        readonly ReceiveObservable _receiveObservers;
        readonly ISendEndpointProvider _sendEndpointProvider;

        BusHandle _busHandle;

        public MassTransitBus(Uri address, IConsumePipe consumePipe, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider, IEnumerable<IReceiveEndpoint> receiveEndpoints, IEnumerable<IBusHostControl> hosts,
            IBusObserver busObservable)
        {
            Address = address;
            _consumePipe = consumePipe;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _busObservable = busObservable;
            _receiveEndpoints = receiveEndpoints.ToArray();
            _hosts = hosts.ToArray();

            _receiveObservers = new ReceiveObservable();
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

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return new MultipleConnectHandle(_receiveEndpoints.Select(x => x.ConnectConsumeMessageObserver(observer)));
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return new MultipleConnectHandle(_receiveEndpoints.Select(x => x.ConnectConsumeObserver(observer)));
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
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
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

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        BusHandle IBusControl.Start()
        {
            return TaskUtil.Await(() => StartAsync(CancellationToken.None));
        }

        public async Task<BusHandle> StartAsync(CancellationToken cancellationToken)
        {
            if (_busHandle != null)
            {
                _log.Warn($"The bus was already started, additional Start attempts are ignored: {Address}");
                return _busHandle;
            }

            await _busObservable.PreStart(this).ConfigureAwait(false);

            Handle busHandle = null;

            var endpoints = new List<ReceiveEndpointHandle>();
            var hosts = new List<HostHandle>();
            var observers = new List<ConnectHandle>();
            var busReady = new BusReady(_receiveEndpoints);
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Starting bus hosts...");

                foreach (var host in _hosts)
                {
                    var hostHandle = host.Start();

                    hosts.Add(hostHandle);
                }

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Starting receive endpoints...");

                foreach (var endpoint in _receiveEndpoints)
                {
                    var observerHandle = endpoint.ConnectReceiveObserver(_receiveObservers);
                    observers.Add(observerHandle);

                    var handle = endpoint.Start();

                    endpoints.Add(handle);
                }

                busHandle = new Handle(hosts, endpoints, observers, this, _busObservable, busReady);

                await busHandle.Ready.WithCancellation(cancellationToken).ConfigureAwait(false);

                await _busObservable.PostStart(this, busReady.Ready).ConfigureAwait(false);

                _busHandle = busHandle;

                return _busHandle;
            }
            catch (Exception ex)
            {
                try
                {
                    if (busHandle != null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Stopping bus hosts...");

                        await busHandle.StopAsync(cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        var handle = new Handle(hosts, endpoints, observers, this, _busObservable, busReady);

                        await handle.StopAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception stopException)
                {
                    _log.Error("Failed to stop partially created bus", stopException);
                }

                await _busObservable.StartFaulted(this, ex).ConfigureAwait(false);

                throw;
            }
        }

        void IBusControl.Stop(CancellationToken cancellationToken)
        {
            if (_busHandle == null)
            {
                _log.Warn($"The bus could not be stopped as it was never started: {Address}");
                return;
            }

            _busHandle.Stop(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (_busHandle == null)
            {
                _log.Warn($"The bus could not be stopped as it was never started: {Address}");
                return TaskUtil.Completed;
            }

            return _busHandle.StopAsync(cancellationToken);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpointProvider.ConnectPublishObserver(observer);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("bus");
            scope.Set(new
            {
                Address
            });

            foreach (var host in _hosts)
                host.Probe(scope);

            foreach (var receiveEndpoint in _receiveEndpoints)
                receiveEndpoint.Probe(scope);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return new MultipleConnectHandle(_receiveEndpoints.Select(x => x.ConnectReceiveEndpointObserver(observer)));
        }

        void IDisposable.Dispose()
        {
            _busHandle?.Stop(CancellationToken.None);

            (_sendEndpointProvider as IDisposable)?.Dispose();
            (_publishEndpointProvider as IDisposable)?.Dispose();
        }


        class BusReady
        {
            readonly ReadyObserver[] _observers;

            public BusReady(IEnumerable<IReceiveEndpoint> receiveEndpoints)
            {
                _observers = receiveEndpoints.Select(x => new ReadyObserver(x)).ToArray();
            }

            public Task<ReceiveEndpointReady[]> Ready
            {
                get { return ReadyOrNot(_observers.Select(x => x.Ready)); }
            }

            async Task<ReceiveEndpointReady[]> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> observers)
            {
                var tasks = observers as Task<ReceiveEndpointReady>[] ?? observers.ToArray();

                foreach (Task<ReceiveEndpointReady> observer in tasks)
                {
                    await observer.ConfigureAwait(false);
                }

                return await Task.WhenAll(tasks).ConfigureAwait(false);
            }


            class ReadyObserver :
                IReceiveEndpointObserver
            {
                readonly ConnectHandle _handle;
                readonly TaskCompletionSource<ReceiveEndpointReady> _ready;

                public ReadyObserver(IReceiveEndpoint endpoint)
                {
                    _ready = new TaskCompletionSource<ReceiveEndpointReady>();
                    _handle = endpoint.ConnectReceiveEndpointObserver(this);
                }

                public Task<ReceiveEndpointReady> Ready => _ready.Task;

                Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
                {
                    _ready.TrySetResult(ready);

                    _handle.Disconnect();

                    return TaskUtil.Completed;
                }

                Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
                {
                    return TaskUtil.Completed;
                }

                public Task Faulted(ReceiveEndpointFaulted faulted)
                {
                    _ready.TrySetException(faulted.Exception);

                    _handle.Disconnect();

                    return TaskUtil.Completed;
                }
            }
        }


        class Handle :
            BusHandle
        {
            readonly IBus _bus;
            readonly IBusObserver _busObserver;
            readonly ReceiveEndpointHandle[] _endpointHandles;
            readonly HostHandle[] _hostHandles;
            readonly ConnectHandle[] _observerHandles;
            bool _stopped;

            public Handle(IEnumerable<HostHandle> hostHandles, IEnumerable<ReceiveEndpointHandle> endpointHandles, IEnumerable<ConnectHandle> observerHandles,
                IBus bus, IBusObserver busObserver, BusReady ready)
            {
                _bus = bus;
                _busObserver = busObserver;
                _endpointHandles = endpointHandles.ToArray();
                _hostHandles = hostHandles.ToArray();
                _observerHandles = observerHandles.ToArray();
                Ready = ready.Ready;
            }

            public Task<ReceiveEndpointReady[]> Ready { get; }

            public void Stop(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                TaskUtil.Await(() => StopAsync(cancellationToken), cancellationToken);

                _stopped = true;
            }

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                await _busObserver.PreStop(_bus).ConfigureAwait(false);

                try
                {
                    foreach (var observerHandle in _observerHandles)
                        observerHandle.Disconnect();

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Stopping endpoints...");

                    await Task.WhenAll(_endpointHandles.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Stopping hosts...");

                    await Task.WhenAll(_hostHandles.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);

                    await _busObserver.PostStop(_bus).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await _busObserver.StopFaulted(_bus, exception).ConfigureAwait(false);

                    throw;
                }

                _stopped = true;
            }

            void IDisposable.Dispose()
            {
                Stop(CancellationToken.None);
            }
        }
    }
}