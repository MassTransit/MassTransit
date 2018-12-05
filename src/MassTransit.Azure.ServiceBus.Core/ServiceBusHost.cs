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
namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Pipeline;
    using Settings;
    using Topology;
    using Transports;


    public class ServiceBusHost :
        Supervisor,
        IServiceBusHostControl
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly IReceiveEndpointCollection _receiveEndpoints;

        public ServiceBusHost(IServiceBusHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _receiveEndpoints = new ReceiveEndpointCollection();
            Add(_receiveEndpoints);

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

        public IReceiveEndpointCollection ReceiveEndpoints => _receiveEndpoints;

        Uri IHost.Address => _hostConfiguration.HostAddress;
        IHostTopology IHost.Topology => _hostConfiguration.Topology;

        async Task<HostHandle> IBusHostControl.Start()
        {
            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            return new StartHostHandle(this, handles, NamespaceContextSupervisor, MessagingFactoryContextSupervisor);
        }

        public IRetryPolicy RetryPolicy { get; }
        public ServiceBusHostSettings Settings => _hostConfiguration.Settings;
        public string BasePath { get; }

        public IServiceBusHostTopology Topology => _hostConfiguration.Topology;

        public IMessagingFactoryContextSupervisor MessagingFactoryContextSupervisor { get; }

        public INamespaceContextSupervisor NamespaceContextSupervisor { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "Azure Service Bus",
                _hostConfiguration.HostAddress,
                _hostConfiguration.Settings.OperationTimeout
            });

            _receiveEndpoints.Probe(scope);

            NamespaceContextSupervisor.Probe(scope);

            MessagingFactoryContextSupervisor.Probe(scope);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            var queueName = Topology.CreateTemporaryQueueName("endpoint-");

            return ConnectReceiveEndpoint(queueName, configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return _receiveEndpoints.Start(queueName);
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

        HostReceiveEndpointHandle CreateSubscriptionEndpoint(Action<IServiceBusSubscriptionEndpointConfigurator> configure,
            SubscriptionEndpointSettings settings)
        {
            var configuration = _hostConfiguration.CreateSubscriptionEndpointConfiguration(settings);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            return _receiveEndpoints.Start(settings.Path);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receiveEndpoints.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpoints.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpoints.ConnectSendObserver(observer);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            _receiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context).ConfigureAwait(false);

            await NamespaceContextSupervisor.Stop(context).ConfigureAwait(false);

            await MessagingFactoryContextSupervisor.Stop(context).ConfigureAwait(false);
        }
    }
}