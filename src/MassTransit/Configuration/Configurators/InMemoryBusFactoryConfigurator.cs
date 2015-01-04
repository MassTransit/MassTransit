// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using PipeConfigurators;
    using Transports;


    public class InMemoryBusFactoryConfigurator :
        IInMemoryServiceBusFactoryConfigurator,
        IBusFactory
    {
        readonly InMemoryReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly IList<IInMemoryServiceBusFactorySpecification> _configurators;
        readonly Uri _inputAddress;
        IReceiveTransportProvider _receiveTransportProvider;
        ISendTransportProvider _sendTransportProvider;

        public InMemoryBusFactoryConfigurator()
        {
            string queueName = NewId.Next().ToString("NS");

            _inputAddress = new Uri(string.Format("loopback://localhost/{0}", queueName));

            _configurators = new List<IInMemoryServiceBusFactorySpecification>();
            _busEndpointConfigurator = new InMemoryReceiveEndpointConfigurator(queueName);
        }

        public IBusControl CreateBus()
        {
            if (_receiveTransportProvider == null || _sendTransportProvider == null)
            {
                var transportProvider = new InMemoryTransportCache();

                _receiveTransportProvider = _receiveTransportProvider ?? transportProvider;
                _sendTransportProvider = _sendTransportProvider ?? transportProvider;
            }

            var builder = new InMemoryBusBuilder(_inputAddress, _receiveTransportProvider, _sendTransportProvider);

            foreach (IInMemoryServiceBusFactorySpecification configurator in _configurators)
                configurator.Configure(builder);

            _busEndpointConfigurator.Configure(builder);

            return builder.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _busEndpointConfigurator.Validate().Concat(_configurators.SelectMany(x => x.Validate()));
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> configurator)
        {
            _busEndpointConfigurator.AddPipeSpecification(configurator);
        }

        public void AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            _configurators.Add(new ConfiguratorProxy(configurator));
        }

        public void SetTransportProvider<T>(T transportProvider)
            where T : ISendTransportProvider, IReceiveTransportProvider
        {
            _receiveTransportProvider = transportProvider;
            _sendTransportProvider = transportProvider;
        }

        public void AddBusFactorySpecification(IInMemoryServiceBusFactorySpecification configurator)
        {
            _configurators.Add(configurator);
        }


        class ConfiguratorProxy :
            IInMemoryServiceBusFactorySpecification
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

            public void Configure(IInMemoryBusBuilder builder)
            {
                _configurator.Configure(builder);
            }
        }
    }
}