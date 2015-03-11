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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using PipeConfigurators;


    public class AzureServiceBusBusFactoryConfigurator :
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly IList<IServiceBusHost> _hosts;
        readonly IList<IBusFactorySpecification> _transportSpecifications;
        ServiceBusReceiveEndpointConfigurator _defaultEndpointConfigurator;
        Uri _localAddress;

        public AzureServiceBusBusFactoryConfigurator()
        {
            _hosts = new List<IServiceBusHost>();
            _transportSpecifications = new List<IBusFactorySpecification>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportSpecifications.SelectMany(x => x.Validate());
        }

        public IBusControl CreateBus()
        {
            var builder = new AzureServiceBusBusBuilder(_hosts, _localAddress);

            foreach (IBusFactorySpecification configurator in _transportSpecifications)
                configurator.Configure(builder);

            return builder.Build();
        }

        public IServiceBusHost Host(ServiceBusHostSettings settings)
        {
            var host = new ServiceBusHost(settings);
            _hosts.Add(host);

            // use first host for default host settings :(
            if (_hosts.Count == 1)
            {
                string queueName = string.Format("bus_{0}", NewId.Next().ToString("NS"));

                _defaultEndpointConfigurator = new ServiceBusReceiveEndpointConfigurator(host, queueName)
                {
                    EnableExpress = true,
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5),
                };

                _transportSpecifications.Add(_defaultEndpointConfigurator);

                _localAddress = settings.GetInputAddress(_defaultEndpointConfigurator.QueueDescription);
            }

            return host;
        }

        void IBusFactoryConfigurator.AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _transportSpecifications.Add(configurator);
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> configurator)
        {
            if (_defaultEndpointConfigurator != null)
                _defaultEndpointConfigurator.AddPipeSpecification(configurator);
        }
    }
}