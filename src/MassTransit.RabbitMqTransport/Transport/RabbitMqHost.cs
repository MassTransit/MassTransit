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
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Topology;
    using Transports;


    public class RabbitMqHost :
        Supervisor,
        IRabbitMqHostControl
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        HostHandle _handle;

        public RabbitMqHost(IRabbitMqHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _receiveEndpoints = new ReceiveEndpointCollection();
            Add(_receiveEndpoints);

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<RabbitMqConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new RabbitMqConnectionContextSupervisor(hostConfiguration);
        }

        public async Task<HostHandle> Start()
        {
            if (_handle != null)
                throw new MassTransitException($"The host was already started: {_hostConfiguration.Description}");

            HostReceiveEndpointHandle[] handles = _receiveEndpoints.StartEndpoints();

            _handle = new StartHostHandle(this, handles);

            return _handle;
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            _receiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "RabbitMQ",
                Settings.Host,
                Settings.Port,
                Settings.VirtualHost,
                Settings.Username,
                Password = new string('*', Settings.Password.Length),
                Settings.Heartbeat,
                Settings.Ssl
            });

            if (Settings.Ssl)
            {
                scope.Set(new
                {
                    Settings.SslServerName
                });
            }

            ConnectionContextSupervisor.Probe(scope);

            _receiveEndpoints.Probe(scope);
        }

        public Uri Address => _hostConfiguration.HostAddress;
        IHostTopology IHost.Topology => _hostConfiguration.Topology;

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public RabbitMqHostSettings Settings => _hostConfiguration.Settings;
        public IRabbitMqHostTopology Topology => _hostConfiguration.Topology;

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_hostConfiguration.Topology.CreateTemporaryQueueName("endpoint-"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return _receiveEndpoints.Start(queueName);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receiveEndpoints.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpoints.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpoints.ConnectSendObserver(observer);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context).ConfigureAwait(false);

            await ConnectionContextSupervisor.Stop(context).ConfigureAwait(false);
        }
    }
}