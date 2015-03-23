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
        readonly IList<IPipeSpecification<ConsumeContext>> _endpointPipeSpecifications;

        public AzureServiceBusBusFactoryConfigurator()
        {
            _hosts = new List<IServiceBusHost>();
            _transportSpecifications = new List<IBusFactorySpecification>();
            _endpointPipeSpecifications = new List<IPipeSpecification<ConsumeContext>>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportSpecifications.SelectMany(x => x.Validate());
        }

        public IBusControl CreateBus()
        {
            var builder = new AzureServiceBusBusBuilder(_hosts, _endpointPipeSpecifications);

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
            if (specification == null)
                throw new ArgumentNullException("specification");

            _endpointPipeSpecifications.Add(specification);
        }
    }
}