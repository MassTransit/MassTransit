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
    using System.Threading.Tasks;
    using Configuration;
    using Configuration.Configuration;
    using Configurators;
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
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        HostHandle _handle;

        public AmazonSqsHost(IAmazonSqsHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _receiveEndpoints = new ReceiveEndpointCollection();
            Add(_receiveEndpoints);

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<AmazonSqsTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new AmazonSqsConnectionContextSupervisor(hostConfiguration);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public AmazonSqsHostSettings Settings => _hostConfiguration.Settings;
        public IAmazonSqsHostTopology Topology => _hostConfiguration.Topology;

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "AmazonSQS",
                Settings.Region,
                Settings.AccessKey,
                Password = new string('*', Settings.SecretKey.Length)
            });

            ConnectionContextSupervisor.Probe(scope);

            _receiveEndpoints.Probe(scope);
        }

        public Uri Address => _hostConfiguration.HostAddress;

        IHostTopology IHost.Topology => _hostConfiguration.Topology;

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

        public async Task<HostHandle> Start()
        {
            if (_handle != null)
                throw new MassTransitException($"The host was already started: {_hostConfiguration.HostAddress}");

            HostReceiveEndpointHandle[] handles = _receiveEndpoints.StartEndpoints();

            _handle = new StartHostHandle(this, handles);

            return _handle;
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_hostConfiguration.Topology.CreateTemporaryQueueName("endpoint-"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return _receiveEndpoints.Start(queueName);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            _receiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context).ConfigureAwait(false);

            await ConnectionContextSupervisor.Stop(context).ConfigureAwait(false);
        }
    }
}