// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Text;
    using System.Threading.Tasks;
    using GreenPipes;
    using Integration;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;


    public class RabbitMqHostConfiguration :
        IRabbitMqHostConfiguration
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;
        readonly RabbitMqHostSettings _hostSettings;
        readonly IRabbitMqHostTopology _hostTopology;
        readonly IRabbitMqHostControl _host;

        public RabbitMqHostConfiguration(IRabbitMqBusConfiguration busConfiguration, RabbitMqHostSettings hostSettings, IRabbitMqHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = hostSettings;
            _hostTopology = hostTopology;

            _host = new RabbitMqHost(this);

            Description = FormatDescription(hostSettings);
        }

        public string Description { get; }

        Uri IHostConfiguration.HostAddress => _hostSettings.HostAddress;
        IBusHostControl IHostConfiguration.Host => _host;
        IHostTopology IHostConfiguration.Topology => _hostTopology;

        IRabbitMqBusConfiguration IRabbitMqHostConfiguration.BusConfiguration => _busConfiguration;
        IRabbitMqHostControl IRabbitMqHostConfiguration.Host => _host;

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            var settings = new RabbitMqReceiveSettings(queueName, queueName, _busConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false);

            return new RabbitMqReceiveEndpointConfiguration(this, settings, _busConfiguration.CreateEndpointConfiguration());
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, string exchangeName, bool enableQueue, bool enableExchange)
        {
            var settings = new RabbitMqReceiveSettings(queueName, exchangeName, _busConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false, enableQueue, enableExchange);

            return new RabbitMqReceiveEndpointConfiguration(this, settings, _busConfiguration.CreateEndpointConfiguration());
        }


        public bool Matches(Uri address)
        {
            switch (address.Scheme.ToLowerInvariant())
            {
                case "rabbitmq":
                case "amqp":
                case "rabbitmqs":
                case "amqps":
                    break;

                default:
                    return false;
            }

            var settings = address.GetHostSettings();

            return RabbitMqHostEqualityComparer.Default.Equals(_hostSettings, settings);
        }

        IRabbitMqHostTopology IRabbitMqHostConfiguration.Topology => _hostTopology;
        public bool PublisherConfirmation => _hostSettings.PublisherConfirmation;
        public RabbitMqHostSettings Settings => _hostSettings;

        public IModelContextSupervisor CreateModelContextSupervisor()
        {
            return new ModelContextSupervisor(_host.ConnectionContextSupervisor);
        }

        public ISendTransport CreateSendTransport(IModelContextSupervisor modelContextSupervisor, IFilter<ModelContext> modelFilter, string exchangeName)
        {
            var transport = new RabbitMqSendTransport(modelContextSupervisor, modelFilter, exchangeName);

            _host.Add(transport);

            return transport;
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            var settings = _hostTopology.GetSendSettings(address);

            var brokerTopology = settings.GetBrokerTopology();

            IModelContextSupervisor supervisor = CreateModelContextSupervisor();

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology);

            var transport = CreateSendTransport(supervisor, configureTopologyFilter, settings.ExchangeName);

            return Task.FromResult(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IRabbitMqMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var sendSettings = publishTopology.GetSendSettings();

            var brokerTopology = publishTopology.GetBrokerTopology();

            IModelContextSupervisor supervisor = CreateModelContextSupervisor();

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(sendSettings, brokerTopology);

            var transport = CreateSendTransport(supervisor, configureTopologyFilter, publishTopology.Exchange.ExchangeName);

            return Task.FromResult(transport);
        }

        static string FormatDescription(RabbitMqHostSettings settings)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(settings.Username))
                sb.Append(settings.Username).Append('@');

            sb.Append(settings.Host);

            var actualHost = settings.HostNameSelector?.LastHost;
            if (!string.IsNullOrWhiteSpace(actualHost))
                sb.Append('(').Append(actualHost).Append(')');

            if (settings.Port != -1)
                sb.Append(':').Append(settings.Port);

            if (string.IsNullOrWhiteSpace(settings.VirtualHost))
                sb.Append('/');
            else if (settings.VirtualHost.StartsWith("/"))
                sb.Append(settings.VirtualHost);
            else
                sb.Append("/").Append(settings.VirtualHost);

            return sb.ToString();
        }
    }
}
