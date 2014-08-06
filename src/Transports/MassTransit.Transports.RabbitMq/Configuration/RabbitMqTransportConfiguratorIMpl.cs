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
namespace MassTransit.Transports.RabbitMq.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using Serialization;


    public class RabbitMqTransportConfigurator :
        IRabbitMqTransportConfigurator,
        ITransportBuilder
    {
        readonly RabbitMqReceiveEndpointConfigurator _defaultEndpointConfigurator;
        readonly IList<RabbitMqHostSettings> _hosts;
        readonly RabbitMqPublishSettings _publishSettings;
        readonly IList<ITransportBuilderConfigurator> _transportBuilderConfigurators;
        HostSettings _defaultHostSettings;

        public RabbitMqTransportConfigurator(ITransportSelector selector)
        {
            _hosts = new List<RabbitMqHostSettings>();
            _defaultHostSettings = new HostSettings();
            _defaultEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator(_defaultHostSettings);
            _publishSettings = new RabbitMqPublishSettings();
            _transportBuilderConfigurators = new List<ITransportBuilderConfigurator>();

            selector.SelectTransport(this);
        }

        public void Host(RabbitMqHostSettings settings)
        {
            _hosts.Add(settings);

            // use first host for default host settings :(
            if (_hosts.Count == 1)
            {
                _defaultHostSettings.Host = settings.Host;
                _defaultHostSettings.VirtualHost = settings.VirtualHost;
                _defaultHostSettings.Port = settings.Port;
                _defaultHostSettings.Username = settings.Username;
                _defaultHostSettings.Password = settings.Password;
                _defaultHostSettings.Heartbeat = settings.Heartbeat;
            }
        }

        public void Endpoint(RabbitMqEndpointSettings settings)
        {
            throw new NotImplementedException();
        }

        public void AddTransportBuilderConfigurator(ITransportBuilderConfigurator configurator)
        {
            _transportBuilderConfigurators.Add(configurator);
        }

        public void Mandatory(bool mandatory = true)
        {
            _publishSettings.Mandatory = mandatory;
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

        public IBus Build()
        {
            var builder = new RabbitMqServiceBusBuilder(_hosts, _publishSettings);

            foreach (ITransportBuilderConfigurator configurator in _transportBuilderConfigurators)
                configurator.Configure(builder);

            IBusControl bus = builder.Build();

            bus.Start();


            return bus;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportBuilderConfigurators
                .SelectMany(x => x.Validate())
                .Concat(_defaultEndpointConfigurator.Validate());
        }


        class HostSettings :
            RabbitMqHostSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string VirtualHost { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public ushort Heartbeat { get; set; }
        }
    }


    public class RabbitMqServiceBusBuilder :
        IServiceBusBuilder
    {
        readonly IEnumerable<RabbitMqHostSettings> _hosts;
        readonly PublishSettings _publishSettings;
        readonly IList<IReceiveEndpoint> _receiveEndpoints;
        IMessageDeserializer _messageDeserializer;

        public RabbitMqServiceBusBuilder(IEnumerable<RabbitMqHostSettings> hosts, PublishSettings publishSettings)
        {
            _hosts = hosts;
            _publishSettings = publishSettings;
            _receiveEndpoints = new List<IReceiveEndpoint>();

            _messageDeserializer = new JsonMessageDeserializer(JsonMessageSerializer.Deserializer);
        }

        public void AddReceiveEndpoint(IReceiveEndpoint receiveEndpoint)
        {
            _receiveEndpoints.Add(receiveEndpoint);
        }

        public IMessageDeserializer MessageDeserializer
        {
            get { return _messageDeserializer; }
        }

        public IBusControl Build()
        {
            var inboundPipe = new InboundPipe();

            return new SuperDuperServiceBus(inboundPipe, _receiveEndpoints);
        }
    }
}