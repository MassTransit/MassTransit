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
namespace MassTransit.Transports.InMemory.Configurators
{
    using System;
    using BusConfigurators;
    using Configuration;
    using EndpointSpecifications;
    using MassTransit.Builders;
    using Topology.Configurators;
    using Topology.Topologies;


    public class InMemoryBusFactoryConfigurator :
        BusFactoryConfigurator,
        IInMemoryBusFactoryConfigurator,
        IBusFactory
    {
        readonly IInMemoryEndpointConfiguration _busEndpointConfiguration;
        readonly IInMemoryBusConfiguration _configuration;
        readonly InMemoryHost _inMemoryHost;
        readonly IInMemoryHostConfiguration _inMemoryHostConfiguration;

        public InMemoryBusFactoryConfigurator(IInMemoryBusConfiguration configuration, IInMemoryEndpointConfiguration busEndpointConfiguration,
            Uri baseAddress = null)
            : base(configuration, busEndpointConfiguration)
        {
            _configuration = configuration;
            _busEndpointConfiguration = busEndpointConfiguration;

            TransportConcurrencyLimit = Environment.ProcessorCount;

            var hostTopology = new InMemoryHostTopology(busEndpointConfiguration.Topology);
            var host = new InMemoryHost(configuration, TransportConcurrencyLimit, hostTopology, baseAddress ?? new Uri("loopback://localhost/"));

            _inMemoryHost = host;
            _inMemoryHostConfiguration = _configuration.CreateHostConfiguration(host);
        }

        public IBusControl CreateBus()
        {
            var busQueueName = _inMemoryHost.Topology.CreateTemporaryQueueName("bus-");

            var busReceiveEndpointConfiguration = _configuration.CreateReceiveEndpointConfiguration(busQueueName, _busEndpointConfiguration);

            var builder = new ConfigurationBusBuilder(_configuration, busReceiveEndpointConfiguration, BusObservable);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IInMemoryMessagePublishTopologyConfigurator<T> configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IInMemoryPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public int TransportConcurrencyLimit { private get; set; }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            var configuration = _inMemoryHostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configuration.ConnectConsumerConfigurationObserver(this);
            configuration.ConnectSagaConfigurationObserver(this);
            configuration.ConnectHandlerConfigurationObserver(this);

            configureEndpoint?.Invoke(configuration.Configurator);

            var specification = new ConfigurationReceiveEndpointSpecification(configuration);

            AddReceiveEndpointSpecification(specification);
        }

        void IBusFactoryConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        public IInMemoryHost Host => _inMemoryHost;
    }
}