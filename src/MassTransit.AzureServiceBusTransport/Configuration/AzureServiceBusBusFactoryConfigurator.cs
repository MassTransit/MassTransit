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
    using Microsoft.ServiceBus;
    using PipeConfigurators;


    public class AzureServiceBusBusFactoryConfigurator :
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly HostSettings _defaultHostSettings;
        readonly IList<ServiceBusHostSettings> _hosts;
        readonly IList<IPipeSpecification<ConsumeContext>> _pipeSpecifications;
        readonly IList<IBusFactorySpecification> _transportSpecifications;

        public AzureServiceBusBusFactoryConfigurator()
        {
            _hosts = new List<ServiceBusHostSettings>();
            _defaultHostSettings = new HostSettings();
            _transportSpecifications = new List<IBusFactorySpecification>();

            _pipeSpecifications = new List<IPipeSpecification<ConsumeContext>>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportSpecifications.SelectMany(x => x.Validate());
        }

        public IBusControl CreateBus()
        {
            var builder = new AzureBusBusBuilder(_hosts);

            foreach (IBusFactorySpecification configurator in _transportSpecifications)
                configurator.Configure(builder);

            return builder.Build();
        }

        public void Host(ServiceBusHostSettings settings)
        {
            _hosts.Add(settings);

            // use first host for default host settings :(
            if (_hosts.Count == 1)
                _defaultHostSettings.CopyFrom(settings);
        }

        void IServiceBusFactoryConfigurator.AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _transportSpecifications.Add(configurator);
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> configurator)
        {
            _pipeSpecifications.Add(configurator);
        }


        class HostSettings :
            ServiceBusHostSettings
        {
            public Uri ServiceUri { get; set; }
            public TokenProvider TokenProvider { get; set; }
            public TimeSpan OperationTimeout { get; set; }

            public void CopyFrom(ServiceBusHostSettings settings)
            {
                ServiceUri = settings.ServiceUri;
                TokenProvider = settings.TokenProvider;
                OperationTimeout = settings.OperationTimeout;
            }
        }
    }
}