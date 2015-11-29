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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using EndpointConfigurators;
    using Transports;
    using Transports.InMemory;


    public class InMemoryBusFactoryConfigurator :
        BusFactoryConfigurator,
        IInMemoryBusFactoryConfigurator,
        IBusFactory
    {
        readonly IList<IInMemoryBusFactorySpecification> _configurators;
        readonly IList<IBusHostControl> _hosts;
        int _concurrencyLimit;
        IReceiveTransportProvider _receiveTransportProvider;
        ISendTransportProvider _sendTransportProvider;

        public InMemoryBusFactoryConfigurator()
        {
            _configurators = new List<IInMemoryBusFactorySpecification>();

            _concurrencyLimit = Environment.ProcessorCount;

            _hosts = new List<IBusHostControl>();
        }

        public IBusControl CreateBus()
        {
            if (_receiveTransportProvider == null || _sendTransportProvider == null)
            {
                var transportProvider = new InMemoryTransportCache(_concurrencyLimit);
                _hosts.Add(transportProvider);

                _receiveTransportProvider = _receiveTransportProvider ?? transportProvider;
                _sendTransportProvider = _sendTransportProvider ?? transportProvider;
            }

            var builder = new InMemoryBusBuilder(_receiveTransportProvider, _sendTransportProvider, _hosts.ToArray(), ConsumePipeFactory, SendPipeFactory, PublishPipeFactory);

            foreach (var configurator in _configurators)
                configurator.Apply(builder);

            return builder.Build();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate()
                .Concat(_configurators.SelectMany(x => x.Validate()));
        }

        void IBusFactoryConfigurator.AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            _configurators.Add(new ConfiguratorProxy(configurator));
        }

        public int TransportConcurrencyLimit
        {
            set { _concurrencyLimit = value; }
        }

        void IInMemoryBusFactoryConfigurator.SetTransportProvider<T>(T transportProvider)
        {
            _receiveTransportProvider = transportProvider;
            _sendTransportProvider = transportProvider;
        }

        public void AddBusFactorySpecification(IInMemoryBusFactorySpecification configurator)
        {
            _configurators.Add(configurator);
        }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            var endpointConfigurator = new InMemoryReceiveEndpointConfigurator(queueName);

            configureEndpoint(endpointConfigurator);

            AddBusFactorySpecification(endpointConfigurator);
        }

        void IBusFactoryConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
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