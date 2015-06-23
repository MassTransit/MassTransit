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


    public class MassTransitBus :
        IBusControl
    {
        readonly Uri _address;
        readonly IConsumePipe _consumePipe;
        readonly IBusHostControl[] _hosts;
        readonly ILog _log;
        readonly IPublishEndpoint _publishEndpoint;
        readonly IReceiveEndpoint[] _receiveEndpoints;
        readonly ReceiveObservable _receiveObservers;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public MassTransitBus(Uri address, IConsumePipe consumePipe, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint, IEnumerable<IReceiveEndpoint> receiveEndpoints, IEnumerable<IBusHostControl> hosts)
        {
            _log = Logger.Get<MassTransitBus>();
            _address = address;
            _consumePipe = consumePipe;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _receiveEndpoints = receiveEndpoints.ToArray();
            _hosts = hosts.ToArray();
            _receiveObservers = new ReceiveObservable();
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
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _consumePipe.ConnectConsumeObserver(observer);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
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
            return _publishEndpoint.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Uri Address
        {
            get { return _address; }
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        BusHandle IBusControl.Start()
        {
            Exception exception = null;

            var endpoints = new List<ReceiveEndpointHandle>();
            var hosts = new List<HostHandle>();
            var observers = new List<ObserverHandle>();
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
                        ObserverHandle observerHandle = endpoint.ConnectReceiveObserver(_receiveObservers);
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
                    Task[] observerTasks = observers.Select(x => x.Disconnect()).ToArray();
                    Task[] endpointTasks = endpoints.Select(x => x.Stop()).ToArray();
                    Task[] hostTasks = hosts.Select(x => x.Stop()).ToArray();

                    Task.WaitAll(observerTasks);
                    Task.WaitAll(endpointTasks);
                    Task.WaitAll(hostTasks);
                }
                catch (Exception ex)
                {
                    _log.Error("Failed to stop partially created bus", ex);
                }
                throw new MassTransitException("The service bus could not be started.", exception);
            }

            return new Handle(hosts.ToArray(), endpoints.ToArray(), observers.ToArray());
        }

        public ObserverHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _publishEndpoint.Connect(observer);
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("bus");
            scope.Set(new
            {
                Address,
            });

            foreach (IBusHostControl host in _hosts)
                await host.Probe(scope);

            foreach (IReceiveEndpoint receiveEndpoint in _receiveEndpoints)
                await receiveEndpoint.Probe(scope);
        }


        class Handle :
            BusHandle
        {
            readonly ReceiveEndpointHandle[] _endpointHandles;
            readonly HostHandle[] _hostHandles;
            readonly ObserverHandle[] _observerHandles;
            bool _disposed;
            bool _stopped;

            public Handle(HostHandle[] hostHandles, ReceiveEndpointHandle[] endpointHandles, ObserverHandle[] observerHandles)
            {
                _endpointHandles = endpointHandles;
                _hostHandles = hostHandles;
                _observerHandles = observerHandles;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                Stop(default(CancellationToken)).Wait();

                _disposed = true;
            }

            public async Task Stop(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                await Task.WhenAll(_observerHandles.Select(x => x.Disconnect())).ConfigureAwait(false);
                await Task.WhenAll(_endpointHandles.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);
                await Task.WhenAll(_hostHandles.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);

                _stopped = true;
            }
        }
    }
}