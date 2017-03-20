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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using Topology.Configuration;
    using Transports;
    using Transports.InMemory;
    using Transports.InMemory.Builders;
    using Transports.InMemory.Configuration;
    using Transports.InMemory.Topology;


    public class InMemoryBusFactoryConfigurator :
        BusFactoryConfigurator<IInMemoryBusBuilder>,
        IInMemoryBusFactoryConfigurator,
        IBusFactory
    {
        readonly IInMemoryEndpointConfiguration _configuration;
        readonly BusHostCollection<IBusHostControl> _hosts;
        int _concurrencyLimit;
        InMemoryHost _inMemoryHost;
        ISendTransportProvider _sendTransportProvider;

        public InMemoryBusFactoryConfigurator(IInMemoryEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _concurrencyLimit = Environment.ProcessorCount;

            _hosts = new BusHostCollection<IBusHostControl>();
        }

        InMemoryHost InMemoryHost
        {
            get
            {
                if (_inMemoryHost == null || _sendTransportProvider == null)
                {
                    var host = new InMemoryHost(_concurrencyLimit);
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

        public void SendTopology<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessageSendTopologyConfigurator<T> configurator = _configuration.SendTopology.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void PublishTopology<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            var configurator = _configuration.PublishTopology.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public int TransportConcurrencyLimit
        {
            set { _concurrencyLimit = value; }
        }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            var endpointSpecification = _configuration.CreateConfiguration();

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

        void IInMemoryBusFactoryConfigurator.SetHost(InMemoryHost host)
        {
            if (_inMemoryHost != null)
                throw new ConfigurationException("The host has already been configured");

            _inMemoryHost = host;
            _sendTransportProvider = host;
            _hosts.Add(host);
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