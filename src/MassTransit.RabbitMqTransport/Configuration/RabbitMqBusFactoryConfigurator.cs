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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using PipeConfigurators;


    public class RabbitMqBusFactoryConfigurator :
        IRabbitMqServiceBusFactoryConfigurator,
        IBusFactory
    {
        readonly IList<RabbitMqHost> _hosts;
        readonly IList<IServiceBusFactoryBuilderConfigurator> _transportBuilderConfigurators;
        RabbitMqReceiveEndpointConfigurator _defaultEndpointConfigurator;
        RabbitMqHost _defaultHost;
        Uri _localAddress;

        public RabbitMqBusFactoryConfigurator()
        {
            _hosts = new List<RabbitMqHost>();
            _transportBuilderConfigurators = new List<IServiceBusFactoryBuilderConfigurator>();
        }

        public IBusControl CreateBus()
        {
            var builder = new RabbitMqServiceBusBuilder(_hosts, _localAddress);

            foreach (IServiceBusFactoryBuilderConfigurator configurator in _transportBuilderConfigurators)
                configurator.Configure(builder);

            IBusControl bus = builder.Build();

            return bus;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");

            foreach (ValidationResult result in _transportBuilderConfigurators.SelectMany(x => x.Validate()))
                yield return result;
            foreach (ValidationResult result in _defaultEndpointConfigurator.Validate())
                yield return result;
        }

        public void Host(RabbitMqHostSettings settings)
        {
            var host = new RabbitMqHost(settings);
            _hosts.Add(host);

            // use first host for default host settings :(
            if (_hosts.Count == 1)
            {
                _defaultHost = host;
                string queueName = NewId.Next().ToString("NS");

                _defaultEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator(_defaultHost, queueName);
                _defaultEndpointConfigurator.Exclusive();
                _defaultEndpointConfigurator.Durable(false);
                _defaultEndpointConfigurator.AutoDelete();

                AddServiceBusFactoryBuilderConfigurator(_defaultEndpointConfigurator);

                _localAddress = settings.GetInputAddress(_defaultEndpointConfigurator.Settings);
            }
        }

        public void AddServiceBusFactoryBuilderConfigurator(IServiceBusFactoryBuilderConfigurator configurator)
        {
            _transportBuilderConfigurators.Add(configurator);
        }

        public void Mandatory(bool mandatory = true)
        {
//            _publishSettings.Mandatory = mandatory;
        }

        public void ReceiveEndpoint(RabbitMqHostSettings hostSettings, string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            RabbitMqHost host = _hosts
                .Where(x => x.Settings.Host.Equals(hostSettings.Host, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.Settings.VirtualHost.Equals(hostSettings.VirtualHost, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (host == null)
                throw new EndpointNotFoundException("The host address specified was not configured.");

            var endpointConfigurator = new RabbitMqReceiveEndpointConfigurator(host, queueName);

            configure(endpointConfigurator);

            AddServiceBusFactoryBuilderConfigurator(endpointConfigurator);
        }

        public void AddPipeBuilderConfigurator(IPipeBuilderConfigurator<ConsumeContext> configurator)
        {
            if (_defaultEndpointConfigurator != null)
                _defaultEndpointConfigurator.AddPipeBuilderConfigurator(configurator);
        }

        public void OnPublish<T>(Action<RabbitMqPublishContext<T>> callback)
            where T : class
        {
            throw new NotImplementedException();
        }

        public void OnPublish(Action<RabbitMqPublishContext> callback)
        {
            throw new NotImplementedException();
        }

        Uri GetSourceAddress(RabbitMqHostSettings host, string queueName)
        {
            var builder = new UriBuilder();

            builder.Scheme = "rabbitmq";
            builder.Host = host.Host;
            builder.Port = host.Port;


            builder.Path = host.VirtualHost != "/" ? string.Join("/", host.VirtualHost, queueName) : queueName;

            builder.Query += string.Format("temporary=true&prefetch=4");

            return builder.Uri;
        }
    }
}