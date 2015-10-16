// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Pipeline;
    using Transports;
    using Util;


    public class MassTransitBus :
        IBusControl,
        IDisposable
    {
        readonly IBusObserver _busObservable;
        readonly IConsumePipe _consumePipe;
        readonly IBusHostControl[] _hosts;
        readonly ILog _log;
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

            _log = Logger.Get<MassTransitBus>();
            _receiveObservers = new ReceiveObservable();

            TaskUtil.Await(() => _busObservable.PostCreate(this));
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
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish(message, publishPipe, cancellationToken);
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
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(Address);

            return publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Uri Address { get; }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        BusHandle IBusControl.Start()
        {
            if (_busHandle != null)
            {
                _log.Warn($"The bus was already started, additional Start attempts are ignored: {Address}");
                return _busHandle;
            }

            TaskUtil.Await(() => _busObservable.PreStart(this));

            Exception exception = null;

            var endpoints = new List<ReceiveEndpointHandle>();
            var hosts = new List<HostHandle>();
            var observers = new List<ConnectHandle>();
            var busReady = new BusReady(_receiveEndpoints);
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Starting bus hosts...");

                foreach (IBusHostControl host in _hosts)
                {
                    try
                    {
                        HostHandle hostHandle = host.Start();

                        hosts.Add(hostHandle);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Starting receive endpoints...");

                foreach (IReceiveEndpoint endpoint in _receiveEndpoints)
                {
                    try
                    {
                        ConnectHandle observerHandle = endpoint.ConnectReceiveObserver(_receiveObservers);
                        observers.Add(observerHandle);

                        ReceiveEndpointHandle handle = endpoint.Start();

                        endpoints.Add(handle);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                try
                {
                    var handle = new Handle(hosts, endpoints, observers, this, _busObservable, busReady);

                    handle.Stop(TimeSpan.FromSeconds(60));
                }
                catch (Exception ex)
                {
                    _log.Error("Failed to stop partially created bus", ex);
                }

                TaskUtil.Await(() => _busObservable.StartFaulted(this, exception));

                throw new MassTransitException("The service bus could not be started.", exception);
            }

            _busHandle = new Handle(hosts, endpoints, observers, this, _busObservable, busReady);

            TaskUtil.Await(() => _busObservable.PostStart(this, busReady.Ready));

            return _busHandle;
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
            ProbeContext scope = context.CreateScope("bus");
            scope.Set(new
            {
                Address
            });

            foreach (IBusHostControl host in _hosts)
                host.Probe(scope);

            foreach (IReceiveEndpoint receiveEndpoint in _receiveEndpoints)
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
        }


        class BusReady
        {
            public BusReady()
            {
                Ready = Task.FromResult(new ReceiveEndpointReady[0]);
            }

            public BusReady(IEnumerable<IReceiveEndpoint> receiveEndpoints)
            {
                var observers = receiveEndpoints.Select(x => new ReadyObserver(x).Ready).ToArray();

                Ready = Task.WhenAll(observers);
            }

            public Task<ReceiveEndpointReady[]> Ready { get; }


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

                TaskUtil.Await(() => _busObserver.PreStop(_bus), cancellationToken);

                try
                {
                    foreach (var observerHandle in _observerHandles)
                        observerHandle.Disconnect();

                    TaskUtil.Await(() => Task.WhenAll(_endpointHandles.Select(x => x.Stop(cancellationToken))), cancellationToken);

                    TaskUtil.Await(() => Task.WhenAll(_hostHandles.Select(x => x.Stop(cancellationToken))), cancellationToken);

                    TaskUtil.Await(() => _busObserver.PostStop(_bus), cancellationToken);
                }
                catch (Exception exception)
                {
                    TaskUtil.Await(() => _busObserver.StopFaulted(_bus, exception), cancellationToken);

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