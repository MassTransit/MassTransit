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
        readonly IList<IServiceBusFactoryBuilderConfigurator> _transportBuilderConfigurators;
        IList<IPipeBuilderConfigurator<ConsumeContext>> _pipeBuilderConfigurators   ;

        public AzureServiceBusBusFactoryConfigurator()
        {
            _hosts = new List<ServiceBusHostSettings>();
            _defaultHostSettings = new HostSettings();
            _transportBuilderConfigurators = new List<IServiceBusFactoryBuilderConfigurator>();

            _pipeBuilderConfigurators = new List<IPipeBuilderConfigurator<ConsumeContext>>();
        }

        public void Host(ServiceBusHostSettings settings)
        {
            _hosts.Add(settings);

            // use first host for default host settings :(
            if (_hosts.Count == 1)
                _defaultHostSettings.CopyFrom(settings);
        }

        void IServiceBusFactoryConfigurator.AddServiceBusFactoryBuilderConfigurator(IServiceBusFactoryBuilderConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _transportBuilderConfigurators.Add(configurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportBuilderConfigurators.SelectMany(x => x.Validate());
        }

        public IBusControl CreateBus()
        {
            var builder = new AzureServiceBusServiceBusBuilder(_hosts);

            foreach (IServiceBusFactoryBuilderConfigurator configurator in _transportBuilderConfigurators)
                configurator.Configure(builder);

            return builder.Build();
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


        void IPipeConfigurator<ConsumeContext>.AddPipeBuilderConfigurator(IPipeBuilderConfigurator<ConsumeContext> configurator)
        {
            _pipeBuilderConfigurators.Add(configurator);

        }
    }
}