// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Pipeline;
    using Policies;
    using Transports;
    using Util;


    public class ReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<ReceiveTransport>();
        readonly IClientCache _clientCache;
        readonly IPipe<ClientContext> _clientPipe;
        readonly IServiceBusHost _host;
        readonly ReceiveObservable _observers;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly ClientSettings _settings;
        readonly ReceiveTransportObservable _transportObservers;

        public ReceiveTransport(IServiceBusHost host, ClientSettings settings, IPublishEndpointProvider publishEndpointProvider,
            ISendEndpointProvider sendEndpointProvider, IClientCache clientCache, IPipe<ClientContext> clientPipe, ReceiveTransportObservable transportObserver)
        {
            _host = host;
            _settings = settings;
            _publishEndpointProvider = publishEndpointProvider;
            _sendEndpointProvider = sendEndpointProvider;
            _clientCache = clientCache;
            _clientPipe = clientPipe;

            _observers = new ReceiveObservable();
            _transportObservers = transportObserver;
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
            var inputAddress = _settings.GetInputAddress(_host.Settings.ServiceUri, _settings.Path);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting receive transport: {0}", inputAddress);

            Task.Factory.StartNew(Receiver, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            return new Handle(this);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _observers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _transportObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpointProvider.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            var sendHandle = _sendEndpointProvider.ConnectSendObserver(observer);
            var publishHandle = _publishEndpointProvider.ConnectSendObserver(observer);

            return new MultipleConnectHandle(sendHandle, publishHandle);
        }

        async Task Receiver()
        {
            var inputAddress = _settings.GetInputAddress(_host.Settings.ServiceUri, _settings.Path);

            while (!IsStopping)
            {
                await _host.RetryPolicy.Retry(async () =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Connecting receive transport: {0}", inputAddress);

                    try
                    {
                        await _clientCache.Send(_clientPipe, Stopped).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"ReceiveTransport Faulted: {inputAddress}", ex);

                        await _transportObservers.Faulted(new ReceiveTransportFaultedEvent(inputAddress, ex)).ConfigureAwait(false);

                        throw;
                    }
                }, Stopping);
            }
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