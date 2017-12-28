// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using BusConfigurators;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology.Configurators;
    using Topology.Topologies;
    using Transports;


    public class InMemoryBusFactoryConfigurator :
        BusFactoryConfigurator<IInMemoryBusBuilder>,
        IInMemoryBusFactoryConfigurator,
        IBusFactory
    {
        readonly Uri _baseAddress;
        readonly IInMemoryEndpointConfiguration _configuration;
        readonly BusHostCollection<IBusHostControl> _hosts;
        InMemoryHost _inMemoryHost;
        ISendTransportProvider _sendTransportProvider;

        public InMemoryBusFactoryConfigurator(IInMemoryEndpointConfiguration configuration, Uri baseAddress = null)
            : base(configuration)
        {
            _configuration = configuration;
            _baseAddress = baseAddress;
            TransportConcurrencyLimit = Environment.ProcessorCount;

            _baseAddress = baseAddress ?? new Uri("loopback://localhost/");

            _hosts = new BusHostCollection<IBusHostControl>();
        }

        InMemoryHost InMemoryHost
        {
            get
            {
                if (_inMemoryHost == null || _sendTransportProvider == null)
                {
                    var hostTopology = new InMemoryHostTopology(_configuration.Topology);
                    var host = new InMemoryHost(TransportConcurrencyLimit, hostTopology, _baseAddress);
                    _hosts.Add(host);

                    _inMemoryHost = _inMemoryHost ?? host;
                    _sendTransportProvider = _sendTransportProvider ?? host;
                }

                return _inMemoryHost;
            }
        }

        ISendTransportProvider SendTransportProvider => _sendTransportProvider ?? (_sendTransportProvider = InMemoryHost);

        public IBusControl CreateBus()
        {
            var builder = new InMemoryBusBuilder(InMemoryHost, SendTransportProvider, _hosts, _configuration);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            var configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IInMemoryPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public int TransportConcurrencyLimit { get; set; }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            var endpointSpecification = _configuration.CreateNewConfiguration();

            var specification = new InMemoryReceiveEndpointSpecification(InMemoryHost.Address, queueName, SendTransportProvider, endpointSpecification);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            configureEndpoint?.Invoke(specification);

            AddReceiveEndpointSpecification(specification);
        }

        void IBusFactoryConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        void IInMemoryBusFactoryConfigurator.SetHost(IInMemoryHost host)
        {
            if (_inMemoryHost != null)
                throw new ConfigurationException("The host has already been configured");

            _inMemoryHost = host as InMemoryHost;
            _sendTransportProvider = _inMemoryHost;
            _hosts.Add(_inMemoryHost);
        }

        public IInMemoryHost Host => InMemoryHost;

        protected override IBusFactorySpecification<IInMemoryBusBuilder> CreateSpecificationProxy(IBusFactorySpecification specification)
        {
            return new ConfiguratorProxy(specification);
        }


        class ConfiguratorProxy :
            IInMemoryBusFactorySpecification
        {
            readonly IBusFactorySpecification _configurator;

            public ConfiguratorProxy(IBusFactorySpecification configurator)
            {
                _configurator = configurator;
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _configurator.Validate();
            }

            public void Apply(IInMemoryBusBuilder builder)
            {
                _configurator.Apply(builder);
            }
        }
    }
}