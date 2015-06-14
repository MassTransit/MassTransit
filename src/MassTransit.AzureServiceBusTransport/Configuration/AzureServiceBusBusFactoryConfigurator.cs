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
    using BusConfigurators;
    using Configurators;
    using PipeConfigurators;


    public class AzureServiceBusBusFactoryConfigurator :
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly ConsumePipeSpecificationList _consumePipeSpecification;
        readonly IList<ServiceBusHost> _hosts;
        readonly IList<IBusFactorySpecification> _transportSpecifications;

        public AzureServiceBusBusFactoryConfigurator()
        {
            _hosts = new List<ServiceBusHost>();
            _transportSpecifications = new List<IBusFactorySpecification>();
            _consumePipeSpecification = new ConsumePipeSpecificationList();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportSpecifications.SelectMany(x => x.Validate());
        }

        public IBusControl CreateBus()
        {
            var builder = new ServiceBusBusBuilder(_hosts, _consumePipeSpecification);

            foreach (IBusFactorySpecification configurator in _transportSpecifications)
                configurator.Apply(builder);

            return builder.Build();
        }

        public IServiceBusHost Host(ServiceBusHostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            var host = new ServiceBusHost(settings);
            _hosts.Add(host);

            return host;
        }

        void IBusFactoryConfigurator.AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _transportSpecifications.Add(configurator);
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _consumePipeSpecification.Add(specification);
        }
    }
}