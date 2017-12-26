// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Settings;
    using Topology;
    using Transport;
    using Transports;
    using Util;


    public class ServiceBusHost :
        IServiceBusHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<ServiceBusHost>();
        readonly Lazy<Task<MessagingFactory>> _messagingFactory;
        readonly Lazy<NamespaceManager> _namespaceManager;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        readonly Lazy<NamespaceManager> _rootNamespaceManager;
        readonly Lazy<Task<MessagingFactory>> _sessionMessagingFactory;
        readonly TaskSupervisor _supervisor;

        public ServiceBusHost(ServiceBusHostSettings settings, IServiceBusHostTopology hostTopology)
        {
            Settings = settings;
            Topology = hostTopology;

            _messagingFactory = new Lazy<Task<MessagingFactory>>(CreateMessagingFactory);
            _sessionMessagingFactory = new Lazy<Task<MessagingFactory>>(CreateNetMessagingFactory);
            _namespaceManager = new Lazy<NamespaceManager>(CreateNamespaceManager);
            _rootNamespaceManager = new Lazy<NamespaceManager>(CreateRootNamespaceManager);
            _receiveEndpoints = new ReceiveEndpointCollection();

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<ServiceBusHost>.ShortName} - {Settings.ServiceUri}");

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

                x.Intervals(100, 500, 1000, 5000, 10000);
            });
        }

        public IServiceBusReceiveEndpointFactory ReceiveEndpointFactory { private get; set; }
        public IServiceBusSubscriptionEndpointFactory SubscriptionEndpointFactory { private get; set; }
        public IReceiveEndpointCollection ReceiveEndpoints => _receiveEndpoints;

        public async Task<HostHandle> Start()
        {
            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            return new Handle(this, _supervisor, handles);
        }

        public bool Matches(Uri address)
        {
            return Settings.ServiceUri.GetLeftPart(UriPartial.Authority).Equals(address.GetLeftPart(UriPartial.Authority),
                StringComparison.OrdinalIgnoreCase);
        }

        public IRetryPolicy RetryPolicy { get; }
        Task<MessagingFactory> IServiceBusHost.SessionMessagingFactory => _sessionMessagingFactory.Value;
        public ServiceBusHostSettings Settings { get; }

        public IServiceBusHostTopology Topology { get; }
        IHostTopology IHost.Topology => Topology;

        Task<MessagingFactory> IServiceBusHost.MessagingFactory => _messagingFactory.Value;
        public NamespaceManager NamespaceManager => _namespaceManager.Value;
        public NamespaceManager RootNamespaceManager => _rootNamespaceManager.Value;
        public ITaskSupervisor Supervisor => _supervisor;

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

        public string GetQueuePath(QueueDescription queueDescription)
        {
            IEnumerable<string> segments = new[] {Settings.ServiceUri.AbsolutePath.Trim('/'), queueDescription.Path.Trim('/')}
                .Where(x => x.Length > 0);

            return string.Join("/", segments);
        }

        public async Task<QueueDescription> CreateQueue(QueueDescription queueDescription)
        {
            var create = true;
            try
            {
                queueDescription = await NamespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);

                create = false;
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating queue {0}", queueDescription.Path);

                    queueDescription = await NamespaceManager.CreateQueueAsync(queueDescription).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                    {
                        throw;
                    }
                }

                if (!created)
                    queueDescription = await NamespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Queue: {0} ({1})", queueDescription.Path,
                    string.Join(", ", new[]
                    {
                        queueDescription.EnableExpress ? "express" : "",
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : "",
                        queueDescription.RequiresSession ? "session" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return queueDescription;
        }

        public async Task<TopicDescription> CreateTopic(TopicDescription topicDescription)
        {
            var create = true;
            try
            {
                topicDescription = await RootNamespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);

                create = false;
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating topic {0}", topicDescription.Path);

                    topicDescription = await RootNamespaceManager.CreateTopicAsync(topicDescription).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                    {
                        throw;
                    }
                }

                if (!created)
                    topicDescription = await RootNamespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Topic: {0} ({1})", topicDescription.Path,
                    string.Join(", ", new[]
                    {
                        topicDescription.EnableExpress ? "express" : "",
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return topicDescription;
        }

        public async Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription description)
        {
            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await RootNamespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(description.ForwardTo))
                {
                    if (!string.IsNullOrWhiteSpace(subscriptionDescription.ForwardTo))
                    {
                        if (_log.IsWarnEnabled)
                            _log.WarnFormat("Removing invalid subscription: {0} ({1} -> {2})", subscriptionDescription.Name,
                                subscriptionDescription.TopicPath,
                                subscriptionDescription.ForwardTo);

                        await RootNamespaceManager.DeleteSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                    }
                }
                else
                {
                    if (description.ForwardTo.Equals(subscriptionDescription.ForwardTo))
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Updating subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                                subscriptionDescription.ForwardTo);

                        await RootNamespaceManager.UpdateSubscriptionAsync(description).ConfigureAwait(false);

                        create = false;
                    }
                    else
                    {
                        if (_log.IsWarnEnabled)
                            _log.WarnFormat("Removing invalid subscription: {0} ({1} -> {2})", subscriptionDescription.Name,
                                subscriptionDescription.TopicPath,
                                subscriptionDescription.ForwardTo);

                        await RootNamespaceManager.DeleteSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                    }
                }
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating subscription {0} -> {1}", description.TopicPath, description.ForwardTo);


                    subscriptionDescription = await RootNamespaceManager.CreateSubscriptionAsync(description).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                    {
                        throw;
                    }
                }

                if (!created)
                    subscriptionDescription = await RootNamespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                    subscriptionDescription.ForwardTo);

            return subscriptionDescription;
        }

        public async Task DeleteTopicSubscription(SubscriptionDescription description)
        {
            try
            {
                await RootNamespaceManager.DeleteSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscription Deleted: {0} ({1} -> {2})", description.Name, description.TopicPath, description.ForwardTo);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            var queueName = this.GetTemporaryQueueName("endpoint");

            return ConnectReceiveEndpoint(queueName, configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            if (ReceiveEndpointFactory == null)
                throw new ConfigurationException("The receive endpoint factory was not specified");

            ReceiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return _receiveEndpoints.Start(queueName);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            if (SubscriptionEndpointFactory == null)
                throw new ConfigurationException("The subscription endpoint factory was not specified");

            var settings = new SubscriptionEndpointSettings(Topology.Publish<T>().TopicDescription, subscriptionName);

            SubscriptionEndpointFactory.CreateSubscriptionEndpoint(settings, configure);

            return _receiveEndpoints.Start(settings.Path);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            if (SubscriptionEndpointFactory == null)
                throw new ConfigurationException("The subscription endpoint factory was not specified");

            var settings = new SubscriptionEndpointSettings(topicName, subscriptionName);

            SubscriptionEndpointFactory.CreateSubscriptionEndpoint(settings, configure);

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

        Task<MessagingFactory> CreateMessagingFactory()
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = Settings.TokenProvider,
                OperationTimeout = Settings.OperationTimeout,
                TransportType = Settings.TransportType
            };

            switch (Settings.TransportType)
            {
                case TransportType.NetMessaging:
                    mfs.NetMessagingTransportSettings = Settings.NetMessagingTransportSettings;
                    break;
                case TransportType.Amqp:
                    mfs.AmqpTransportSettings = Settings.AmqpTransportSettings;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return CreateFactory(mfs);
        }

        async Task<MessagingFactory> CreateFactory(MessagingFactorySettings mfs)
        {
            var builder = new UriBuilder(Settings.ServiceUri) {Path = ""};

            var messagingFactory = await MessagingFactory.CreateAsync(builder.Uri, mfs).ConfigureAwait(false);

            messagingFactory.RetryPolicy = new RetryExponential(Settings.RetryMinBackoff, Settings.RetryMaxBackoff, Settings.RetryLimit);

            return messagingFactory;
        }

        Task<MessagingFactory> CreateNetMessagingFactory()
        {
            if (Settings.TransportType == TransportType.NetMessaging)
                return _messagingFactory.Value;

            var mfs = new MessagingFactorySettings
            {
                TokenProvider = Settings.TokenProvider,
                OperationTimeout = Settings.OperationTimeout,
                TransportType = TransportType.NetMessaging,
                NetMessagingTransportSettings = Settings.NetMessagingTransportSettings
            };

            return CreateFactory(mfs);
        }

        NamespaceManager CreateNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = Settings.TokenProvider,
                OperationTimeout = Settings.OperationTimeout
            };

            return new NamespaceManager(Settings.ServiceUri, nms);
        }

        NamespaceManager CreateRootNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = Settings.TokenProvider,
                OperationTimeout = Settings.OperationTimeout
            };
            var builder = new UriBuilder(Settings.ServiceUri)
            {
                Path = ""
            };

            return new NamespaceManager(builder.Uri, nms);
        }


        class Handle :
            BaseHostHandle
        {
            readonly ServiceBusHost _host;
            readonly TaskSupervisor _supervisor;

            public Handle(ServiceBusHost host, TaskSupervisor supervisor, HostReceiveEndpointHandle[] handles)
                : base(host, handles)
            {
                _host = host;
                _supervisor = supervisor;
            }

            public override async Task Stop(CancellationToken cancellationToken)
            {
                await base.Stop(cancellationToken).ConfigureAwait(false);

                try
                {
                    await _supervisor.Stop("Host stopped", cancellationToken).ConfigureAwait(false);

                    if (_host._messagingFactory.IsValueCreated)
                    {
                        var factory = await _host._messagingFactory.Value.ConfigureAwait(false);

                        if (!factory.IsClosed)
                            await factory.CloseAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                        _log.Warn("Exception closing messaging factory", ex);
                }

                if (_host._sessionMessagingFactory.IsValueCreated && _host.Settings.TransportType == TransportType.Amqp)
                    try
                    {
                        var factory = await _host._sessionMessagingFactory.Value.ConfigureAwait(false);

                        if (!factory.IsClosed)
                            await factory.CloseAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsWarnEnabled)
                            _log.Warn("Exception closing messaging factory", ex);
                    }
            }
        }
    }
}