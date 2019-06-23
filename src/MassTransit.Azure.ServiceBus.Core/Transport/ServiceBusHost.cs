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
namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using Configuration;
    using Definition;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Configurators;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Pipeline;
    using Settings;
    using Topology;
    using Transports;


    public class ServiceBusHost :
        BaseHost,
        IServiceBusHostControl
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusHost(IServiceBusHostConfiguration hostConfiguration)
            : base(hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            RetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Ignore<MessagingEntityNotFoundException>();
                x.Ignore<MessagingEntityAlreadyExistsException>();
                x.Ignore<MessageNotFoundException>();
                x.Ignore<MessageSizeExceededException>();

                x.Handle<ServerBusyException>(exception => exception.IsTransient);
                x.Handle<TimeoutException>();

                x.Interval(5, TimeSpan.FromSeconds(10));
            });

            BasePath = _hostConfiguration.HostAddress.AbsolutePath.Trim('/');

            MessagingFactoryContextSupervisor = new MessagingFactoryContextSupervisor(hostConfiguration);

            NamespaceContextSupervisor = new NamespaceContextSupervisor(hostConfiguration);
        }

        public IRetryPolicy RetryPolicy { get; }
        public ServiceBusHostSettings Settings => _hostConfiguration.Settings;
        public string BasePath { get; }

        public IServiceBusHostTopology Topology => _hostConfiguration.Topology;

        public IMessagingFactoryContextSupervisor MessagingFactoryContextSupervisor { get; }

        public INamespaceContextSupervisor NamespaceContextSupervisor { get; }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "Azure Service Bus",
                _hostConfiguration.HostAddress,
                _hostConfiguration.Settings.OperationTimeout
            });

            NamespaceContextSupervisor.Probe(context);

            MessagingFactoryContextSupervisor.Probe(context);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            var settings = new SubscriptionEndpointSettings(Topology.Publish<T>().TopicDescription, subscriptionName);

            return CreateSubscriptionEndpoint(configure, settings);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            var settings = new SubscriptionEndpointSettings(topicName, subscriptionName);

            return CreateSubscriptionEndpoint(configure, settings);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {NamespaceContextSupervisor, MessagingFactoryContextSupervisor};
        }

        HostReceiveEndpointHandle CreateSubscriptionEndpoint(Action<IServiceBusSubscriptionEndpointConfigurator> configure,
            SubscriptionEndpointSettings settings)
        {
            var configuration = _hostConfiguration.CreateSubscriptionEndpointConfiguration(settings);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return ReceiveEndpoints.Start(settings.Path);
        }
    }
}
