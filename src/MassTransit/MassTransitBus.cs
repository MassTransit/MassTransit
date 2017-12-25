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
    using Events;
    using GreenPipes;
    using Internals.Extensions;
    using Logging;
    using Pipeline;
    using Topology;
    using Transports;
    using Util;


    public class MassTransitBus :
        IBusControl,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<MassTransitBus>();
        readonly IBusObserver _busObservable;
        readonly IConsumePipe _consumePipe;
        readonly IBusHostCollection _hosts;
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly ISendEndpointProvider _sendEndpointProvider;
        Handle _busHandle;

        public MassTransitBus(Uri address, IConsumePipe consumePipe, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider, IBusHostCollection hosts,
            IBusObserver busObservable, IBusTopology topology)
        {
            Address = address;
            _consumePipe = consumePipe;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _busObservable = busObservable;
            Topology = topology;
            _hosts = hosts;

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

        public IBusTopology Topology { get; }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
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

            var hosts = new List<HostHandle>();
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Starting bus hosts...");

                foreach (var host in _hosts)
                {
                    var hostHandle = await host.Start().ConfigureAwait(false);

                    hosts.Add(hostHandle);
                }

                busHandle = new Handle(hosts, this, _busObservable);

                await busHandle.Ready.WithCancellation(cancellationToken).ConfigureAwait(false);

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
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Stopping bus hosts...");

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
                    _log.Error("Failed to stop partially created bus", stopException);
                }

                await _busObservable.StartFaulted(this, ex).ConfigureAwait(false);

                throw;
            }
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
            scope.Set(new
            {
                Address
            });

            foreach (var host in _hosts)
                host.Probe(scope);
        }

        void IDisposable.Dispose()
        {
            if (_busHandle != null && !_busHandle.Stopped)
                throw new MassTransitException("The bus was disposed without being stopped. Explicitly call StopAsync before the bus instance is disposed.");

            (_sendEndpointProvider as IDisposable)?.Dispose();
            (_publishEndpointProvider as IDisposable)?.Dispose();
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

            public bool Stopped => _stopped;

            public Task<BusReady> Ready => ReadyOrNot(_hostHandles.Select(x => x.Ready));

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                await _busObserver.PreStop(_bus).ConfigureAwait(false);

                try
                {
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