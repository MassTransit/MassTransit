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
    using EndpointConfigurators;
    using GreenPipes;
    using Transports;
    using Transports.InMemory;


    public class InMemoryBusFactoryConfigurator :
        BusFactoryConfigurator<IInMemoryBusBuilder>,
        IInMemoryBusFactoryConfigurator,
        IBusFactory
    {
        readonly BusHostCollection<IBusHostControl> _hosts;
        int _concurrencyLimit;
        InMemoryHost _inMemoryHost;
        ISendTransportProvider _sendTransportProvider;

        public InMemoryBusFactoryConfigurator()
        {
            _concurrencyLimit = Environment.ProcessorCount;

            _hosts = new BusHostCollection<IBusHostControl>();
        }

        public IBusControl CreateBus()
        {
            if (_inMemoryHost == null || _sendTransportProvider == null)
            {
                var transportProvider = new InMemoryHost(_concurrencyLimit);
                _hosts.Add(transportProvider);

                _inMemoryHost = _inMemoryHost ?? transportProvider;
                _sendTransportProvider = _sendTransportProvider ?? transportProvider;
            }

            var builder = new InMemoryBusBuilder(_inMemoryHost, _sendTransportProvider, _hosts, ConsumePipeFactory, SendPipeFactory, PublishPipeFactory);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public int TransportConcurrencyLimit
        {
            set { _concurrencyLimit = value; }
        }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            var specification = new InMemoryReceiveEndpointSpecification(queueName);

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
            _inMemoryHost = host;
            _sendTransportProvider = host;
            _hosts.Add(host);
        }

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