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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Pipeline;
    using Settings;
    using Topology;
    using Transport;
    using Transports;


    public class ServiceBusHost :
        Supervisor,
        IServiceBusHost,
        IBusHostControl
    {
        readonly IServiceBusReceiveEndpointFactory _receiveEndpointFactory;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        readonly IServiceBusSubscriptionEndpointFactory _subscriptionEndpointFactory;

        public ServiceBusHost(ServiceBusHostSettings settings, IServiceBusHostTopology hostTopology, IServiceBusBusConfiguration busConfiguration)
        {
            Settings = settings;
            Topology = hostTopology;
            var busConfiguration1 = busConfiguration;

            _receiveEndpoints = new ReceiveEndpointCollection();

            RetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Ignore<MessagingEntityNotFoundException>();
                x.Ignore<MessagingEntityAlreadyExistsException>();
                x.Ignore<MessageNotFoundException>();
                x.Ignore<MessageSizeExceededException>();
                x.Ignore<NoMatchingSubscriptionException>();
                x.Ignore<TransactionSizeExceededException>();

                x.Handle<ServerBusyException>(exception => exception.IsTransient || exception.IsWrappedExceptionTransient());
                x.Handle<MessagingException>(exception => exception.IsTransient || exception.IsWrappedExceptionTransient());
                x.Handle<TimeoutException>();

                x.Interval(5, TimeSpan.FromSeconds(10));
            });

            BasePath = settings.ServiceUri.AbsolutePath.Trim('/');

            var serviceBusRetryPolicy = CreateRetryPolicy(settings);

            MessagingFactoryCache = new MessagingFactoryCache(settings.ServiceUri, CreateMessagingFactorySettings(settings), serviceBusRetryPolicy);
            NamespaceCache = new NamespaceCache(settings.ServiceUri, CreateNamespaceManagerSettings(settings, serviceBusRetryPolicy));

            NetMessagingFactoryCache = settings.TransportType == TransportType.NetMessaging
                ? MessagingFactoryCache
                : new MessagingFactoryCache(settings.ServiceUri, CreateMessagingFactorySettings(settings, true), serviceBusRetryPolicy);

            _receiveEndpointFactory = new ServiceBusReceiveEndpointFactory(busConfiguration1, this);
            _subscriptionEndpointFactory = new ServiceBusSubscriptionEndpointFactory(busConfiguration1, this);
        }

        public IReceiveEndpointCollection ReceiveEndpoints => _receiveEndpoints;

        public async Task<HostHandle> Start()
        {
            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            return new Handle(this, handles);
        }

        public bool Matches(Uri address)
        {
            return Settings.ServiceUri.GetLeftPart(UriPartial.Authority).Equals(address.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase);
        }

        public IRetryPolicy RetryPolicy { get; }
        public ServiceBusHostSettings Settings { get; }
        public string BasePath { get; }

        public IServiceBusHostTopology Topology { get; }

        public IMessagingFactoryCache MessagingFactoryCache { get; }

        public IMessagingFactoryCache NetMessagingFactoryCache { get; }

        public INamespaceCache NamespaceCache { get; }

        IHostTopology IHost.Topology => Topology;

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "Azure Service Bus",
                Settings.ServiceUri,
                Settings.OperationTimeout
            });

            _receiveEndpoints.Probe(scope);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            var queueName = Topology.CreateTemporaryQueueName("endpoint-");

            return ConnectReceiveEndpoint(queueName, configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            if (_receiveEndpointFactory == null)
                throw new ConfigurationException("The receive endpoint factory was not specified");

            _receiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return _receiveEndpoints.Start(queueName);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            if (_subscriptionEndpointFactory == null)
                throw new ConfigurationException("The subscription endpoint factory was not specified");

            var settings = new SubscriptionEndpointSettings(Topology.Publish<T>().TopicDescription, subscriptionName);

            _subscriptionEndpointFactory.CreateSubscriptionEndpoint(settings, configure);

            return _receiveEndpoints.Start(settings.Path);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            if (_subscriptionEndpointFactory == null)
                throw new ConfigurationException("The subscription endpoint factory was not specified");

            var settings = new SubscriptionEndpointSettings(topicName, subscriptionName);

            _subscriptionEndpointFactory.CreateSubscriptionEndpoint(settings, configure);

            return _receiveEndpoints.Start(settings.Path);
        }

        public Uri Address => Settings.ServiceUri;

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
            await _receiveEndpoints.Stop(context).ConfigureAwait(false);

            await base.StopSupervisor(context).ConfigureAwait(false);
        }

        static RetryPolicy CreateRetryPolicy(ServiceBusHostSettings settings)
        {
            return new RetryExponential(settings.RetryMinBackoff, settings.RetryMaxBackoff, settings.RetryLimit);
        }

        static MessagingFactorySettings CreateMessagingFactorySettings(ServiceBusHostSettings settings, bool useNetMessaging = false)
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = settings.OperationTimeout
            };

            if (settings.TransportType == TransportType.NetMessaging || useNetMessaging)
            {
                mfs.TransportType = TransportType.NetMessaging;
                mfs.NetMessagingTransportSettings = settings.NetMessagingTransportSettings;
            }
            else
            {
                mfs.TransportType = TransportType.Amqp;
                mfs.AmqpTransportSettings = settings.AmqpTransportSettings;
            }

            return mfs;
        }

        static NamespaceManagerSettings CreateNamespaceManagerSettings(ServiceBusHostSettings settings, RetryPolicy retryPolicy)
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = settings.OperationTimeout,
                RetryPolicy = retryPolicy
            };

            return nms;
        }


        class Handle :
            HostHandle
        {
            readonly HostReceiveEndpointHandle[] _handles;
            readonly ServiceBusHost _host;

            public Handle(ServiceBusHost host, HostReceiveEndpointHandle[] handles)
            {
                _host = host;
                _handles = handles;
            }

            Task<HostReady> HostHandle.Ready
            {
                get { return ReadyOrNot(_handles.Select(x => x.Ready)); }
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
                await Task.WhenAll(_handles.Select(x => x.StopAsync(cancellationToken))).ConfigureAwait(false);

                await _host.Stop("Host Stopped", cancellationToken).ConfigureAwait(false);

                await _host.MessagingFactoryCache.Stop("Host stopped", cancellationToken).ConfigureAwait(false);

                await _host.NamespaceCache.Stop("Host stopped", cancellationToken).ConfigureAwait(false);
            }

            async Task<HostReady> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
            {
                Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();

                foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                    await ready.ConfigureAwait(false);

                await _host.MessagingFactoryCache.Ready.ConfigureAwait(false);

                await _host.NamespaceCache.Ready.ConfigureAwait(false);

                ReceiveEndpointReady[] endpointsReady = await Task.WhenAll(readyTasks).ConfigureAwait(false);

                return new HostReadyEvent(_host.Address, endpointsReady);
            }
        }
    }
}