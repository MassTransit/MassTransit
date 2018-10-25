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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Configuration.Builders;
    using Configuration.Configuration;
    using Configuration.Configurators;
    using Events;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Topology;
    using Transports;


    public class AmazonSqsHost :
        Supervisor,
        IAmazonSqsHostControl
    {
        readonly AmazonSqsHostSettings _settings;
        readonly IAmazonSqsHostTopology _topology;
        HostHandle _handle;

        public AmazonSqsHost(IAmazonSqsBusConfiguration busConfiguration, AmazonSqsHostSettings settings, IAmazonSqsHostTopology topology)
        {
            _settings = settings;
            _topology = topology;

            ReceiveEndpoints = new ReceiveEndpointCollection();

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<AmazonSqsTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionCache = new AmazonSqsConnectionCache(settings, _topology);

            ReceiveEndpointFactory = new AmazonSqsReceiveEndpointFactory(busConfiguration, this);
        }

        public IAmazonSqsReceiveEndpointFactory ReceiveEndpointFactory { get; set; }

        public IReceiveEndpointCollection ReceiveEndpoints { get; }

        public IConnectionCache ConnectionCache { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public AmazonSqsHostSettings Settings => _settings;
        public IAmazonSqsHostTopology Topology => _topology;

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "AmazonSQS",
                _settings.Region,
                _settings.AccessKey,
                Password = new string('*', _settings.SecretKey.Length),
                _settings.ExecutionRoleArn
            });

            ConnectionCache.Probe(scope);

            ReceiveEndpoints.Probe(scope);
        }

        public Uri Address => _settings.HostAddress;

        IHostTopology IHost.Topology => _topology;

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return ReceiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return ReceiveEndpoints.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return ReceiveEndpoints.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return ReceiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return ReceiveEndpoints.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return ReceiveEndpoints.ConnectSendObserver(observer);
        }

        public async Task<HostHandle> Start()
        {
            if (_handle != null)
                throw new MassTransitException($"The host was already started: {_settings}");

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            _handle = new Handle(handles, this, ConnectionCache);

            return _handle;
        }

        public bool Matches(Uri address)
        {
            if (!address.Scheme.Equals("amazonsqs", StringComparison.OrdinalIgnoreCase))
                return false;

            var settings = new AmazonSqsHostConfigurator(address).Settings;

            return AmazonSqsHostEqualityComparer.Default.Equals(_settings, settings);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_topology.CreateTemporaryQueueName("endpoint"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
        {
            if (ReceiveEndpointFactory == null)
                throw new ConfigurationException("The receive endpoint factory was not specified");

            ReceiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return ReceiveEndpoints.Start(queueName);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            ReceiveEndpoints.Add(endpointName, receiveEndpoint);
        }


        class Handle :
            HostHandle
        {
            readonly IAgent _connectionCache;
            readonly HostReceiveEndpointHandle[] _handles;
            readonly AmazonSqsHost _host;

            public Handle(HostReceiveEndpointHandle[] handles, AmazonSqsHost host, IAgent connectionCache)
            {
                _host = host;
                _handles = handles;
                _connectionCache = connectionCache;
            }

            Task<HostReady> HostHandle.Ready
            {
                get { return ReadyOrNot(_handles.Select(x => x.Ready)); }
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
                await Task.WhenAll(_handles.Select(x => x.StopAsync(cancellationToken))).ConfigureAwait(false);

                await _host.Stop("Host Stopped", cancellationToken).ConfigureAwait(false);

                await _connectionCache.Stop("Host stopped", cancellationToken).ConfigureAwait(false);
            }

            async Task<HostReady> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
            {
                Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();

                foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                    await ready.ConfigureAwait(false);

                await _connectionCache.Ready.ConfigureAwait(false);

                ReceiveEndpointReady[] endpointsReady = await Task.WhenAll(readyTasks).ConfigureAwait(false);

                return new HostReadyEvent(_host.Address, endpointsReady);
            }
        }
    }
}