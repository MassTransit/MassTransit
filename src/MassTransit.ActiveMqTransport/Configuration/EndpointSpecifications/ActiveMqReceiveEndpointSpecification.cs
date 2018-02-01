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
namespace MassTransit.ActiveMqTransport.EndpointSpecifications
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using MassTransit.Builders;
    using MassTransit.EndpointSpecifications;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Observables;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;


    public class ActiveMqReceiveEndpointSpecification :
        ReceiveEndpointSpecification,
        IActiveMqReceiveEndpointConfigurator,
        IReceiveEndpointSpecification<IBusBuilder>
    {
        readonly IActiveMqEndpointConfiguration _configuration;
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionContextConfigurator;
        readonly ActiveMqHost _host;
        readonly IBuildPipeConfigurator<SessionContext> _modelContextConfigurator;
        readonly QueueReceiveSettings _settings;
        bool _bindMessageTopics;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        public ActiveMqReceiveEndpointSpecification(ActiveMqHost host, IActiveMqEndpointConfiguration configuration, string queueName = null)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
            _settings = new QueueReceiveSettings(queueName, true, false);

            _bindMessageTopics = true;

            _connectionContextConfigurator = new PipeConfigurator<ConnectionContext>();
            _modelContextConfigurator = new PipeConfigurator<SessionContext>();
        }

        public ActiveMqReceiveEndpointSpecification(ActiveMqHost host, IActiveMqEndpointConfiguration configuration,
            QueueReceiveSettings settings)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
            _settings = settings;

            _connectionContextConfigurator = new PipeConfigurator<ConnectionContext>();
            _modelContextConfigurator = new PipeConfigurator<SessionContext>();
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider;

        IActiveMqHost IActiveMqReceiveEndpointConfigurator.Host => _host;

        public bool Durable
        {
            set
            {
                _settings.Durable = value;

                Changed("Durable");
            }
        }

        public bool AutoDelete
        {
            set
            {
                _settings.AutoDelete = value;

                Changed("AutoDelete");
            }
        }

        public bool PurgeOnStartup
        {
            set => _settings.PurgeOnStartup = value;
        }

        public ushort PrefetchCount
        {
            set
            {
                _settings.PrefetchCount = value;

                Changed("PrefetchCount");
            }
        }

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public bool BindMessageTopics
        {
            set => _bindMessageTopics = value;
        }

        public void Bind(string topicName)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            _configuration.Topology.Consume.Bind(topicName);
        }

        public void Bind<T>()
            where T : class
        {
            _configuration.Topology.Consume.GetMessageTopology<T>().Bind();
        }

        public void Bind(string topicName, Action<ITopicBindingConfigurator> callback)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _configuration.Topology.Consume.Bind(topicName, callback);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result.WithParentKey($"{_settings.EntityName}");

            if (!ActiveMqEntityNameValidator.Validator.IsValidEntityName(_settings.EntityName))
                yield return this.Failure($"{_settings.EntityName}", "Is not a valid queue name");

            if (_settings.PurgeOnStartup)
                yield return this.Warning($"{_settings.EntityName}", "Existing messages in the queue will be purged on service start");
        }

        public void Apply(IBusBuilder builder)
        {
            var rabbitMqBusBuilder = builder as ActiveMqBusBuilder;
            if (rabbitMqBusBuilder == null)
                throw new ConfigurationException("Must be a RabbitMqBusBuilder");

            var receiveEndpointBuilder = new ActiveMqReceiveEndpointBuilder(_host, rabbitMqBusBuilder.Hosts, _bindMessageTopics, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            _modelContextConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointTopology.BrokerTopology));

            var receiveObserver = new ReceiveObservable();
            var transportObserver = new ReceiveTransportObservable();

            IAgent consumerAgent;
            if (builder.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<SessionContext>(transportObserver, InputAddress);
                _modelContextConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                var deadLetterTransport = CreateDeadLetterTransport();

                var errorTransport = CreateErrorTransport();

                var consumerFilter = new ActiveMqConsumerFilter(receivePipe, receiveObserver, transportObserver, receiveEndpointTopology, deadLetterTransport, errorTransport);
                _modelContextConfigurator.UseFilter(consumerFilter);

                consumerAgent = consumerFilter;
            }


            IFilter<ConnectionContext> modelFilter = new ReceiveSessionFilter(_modelContextConfigurator.Build(), _host);

            _connectionContextConfigurator.UseFilter(modelFilter);

            var transport = new ActiveMqReceiveTransport(_host, _settings, _connectionContextConfigurator.Build(), receiveEndpointTopology, receiveObserver,
                transportObserver);

            transport.Add(consumerAgent);

            _host.ReceiveEndpoints.Add(_settings.EntityName ?? NewId.Next().ToString(), new ReceiveEndpoint(transport, receivePipe));
        }

        IErrorTransport CreateErrorTransport()
        {
            var settings = _configuration.Topology.Send.GetErrorSettings(_settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(settings, settings.GetBrokerTopology());

            return new ActiveMqErrorTransport(settings.EntityName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var settings = _configuration.Topology.Send.GetDeadLetterSettings(_settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(settings, settings.GetBrokerTopology());

            return new ActiveMqDeadLetterTransport(settings.EntityName, filter);
        }

        protected override Uri GetInputAddress()
        {
            return _settings.GetInputAddress(_host.Settings.HostAddress);
        }
    }
}