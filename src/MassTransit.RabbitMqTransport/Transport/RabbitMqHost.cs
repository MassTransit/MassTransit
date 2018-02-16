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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Topology;
    using Transports;


    public class RabbitMqHost :
        Supervisor,
        IRabbitMqHostControl
    {
        readonly RabbitMqHostSettings _settings;
        readonly IRabbitMqHostTopology _topology;
        HostHandle _handle;

        public RabbitMqHost(IRabbitMqBusConfiguration busConfiguration, RabbitMqHostSettings settings, IRabbitMqHostTopology topology)
        {
            _settings = settings;
            _topology = topology;

            ReceiveEndpoints = new ReceiveEndpointCollection();

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<RabbitMqConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionCache = new RabbitMqConnectionCache(settings, _topology);

            ReceiveEndpointFactory = new RabbitMqReceiveEndpointFactory(busConfiguration, this);
        }

        public IRabbitMqReceiveEndpointFactory ReceiveEndpointFactory { get; }

        public IReceiveEndpointCollection ReceiveEndpoints { get; }

        public async Task<HostHandle> Start()
        {
            if (_handle != null)
                throw new MassTransitException($"The host was already started: {_settings.ToDebugString()}");

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            _handle = new Handle(handles, this, ConnectionCache);

            return _handle;
        }

        public bool Matches(Uri address)
        {
            if (!address.Scheme.Equals("rabbitmq", StringComparison.OrdinalIgnoreCase))
                return false;

            var settings = address.GetHostSettings();

            return RabbitMqHostEqualityComparer.Default.Equals(_settings, settings);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            ReceiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "RabbitMQ",
                _settings.Host,
                _settings.Port,
                _settings.VirtualHost,
                _settings.Username,
                Password = new string('*', _settings.Password.Length),
                _settings.Heartbeat,
                _settings.Ssl
            });

            if (_settings.Ssl)
                scope.Set(new
                {
                    _settings.SslServerName
                });

            ConnectionCache.Probe(scope);

            ReceiveEndpoints.Probe(scope);
        }

        public Uri Address => _settings.HostAddress;
        IHostTopology IHost.Topology => _topology;

        public IConnectionCache ConnectionCache { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public RabbitMqHostSettings Settings => _settings;
        public IRabbitMqHostTopology Topology => _topology;

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_topology.CreateTemporaryQueueName("endpoint"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            if (ReceiveEndpointFactory == null)
                throw new ConfigurationException("The receive endpoint factory was not specified");

            ReceiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return ReceiveEndpoints.Start(queueName);
        }

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


        class Handle :
            HostHandle
        {
            readonly IAgent _connectionCache;
            readonly HostReceiveEndpointHandle[] _handles;
            readonly RabbitMqHost _host;

            public Handle(HostReceiveEndpointHandle[] handles, RabbitMqHost host, IAgent connectionCache)
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