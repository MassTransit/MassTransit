// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Logging;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Transports;
    using Util;


    public class ServiceBusHost :
        IServiceBusHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<ServiceBusHost>();
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly Lazy<Task<MessagingFactory>> _messagingFactory;
        readonly Lazy<NamespaceManager> _namespaceManager;
        readonly Lazy<NamespaceManager> _rootNamespaceManager;
        readonly Lazy<Task<MessagingFactory>> _sessionMessagingFactory;
        readonly ServiceBusHostSettings _settings;
        readonly TaskSupervisor _supervisor;

        public ServiceBusHost(ServiceBusHostSettings settings)
        {
            _settings = settings;
            _messagingFactory = new Lazy<Task<MessagingFactory>>(CreateMessagingFactory);
            _sessionMessagingFactory = new Lazy<Task<MessagingFactory>>(CreateNetMessagingFactory);
            _namespaceManager = new Lazy<NamespaceManager>(CreateNamespaceManager);
            _rootNamespaceManager = new Lazy<NamespaceManager>(CreateRootNamespaceManager);

            _messageNameFormatter = new ServiceBusMessageNameFormatter();

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<ServiceBusHost>.ShortName} - {_settings.ServiceUri}");

            RetryPolicy = Retry.Selected<ServerBusyException, TimeoutException>().Intervals(100, 500, 1000, 5000, 10000);
        }

        public HostHandle Start()
        {
            return new Handle(_messagingFactory.Value, _sessionMessagingFactory, _settings, _supervisor);
        }

        public IRetryPolicy RetryPolicy { get; }

        Task<MessagingFactory> IServiceBusHost.SessionMessagingFactory => _sessionMessagingFactory.Value;

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "Azure Service Bus",
                _settings.ServiceUri,
                _settings.OperationTimeout
            });
        }

        ServiceBusHostSettings IServiceBusHost.Settings => _settings;

        Task<MessagingFactory> IServiceBusHost.MessagingFactory => _messagingFactory.Value;

        public NamespaceManager NamespaceManager => _namespaceManager.Value;

        public NamespaceManager RootNamespaceManager => _rootNamespaceManager.Value;

        IMessageNameFormatter IServiceBusHost.MessageNameFormatter => _messageNameFormatter;

        public ITaskSupervisor Supervisor => _supervisor;

        public string GetQueuePath(QueueDescription queueDescription)
        {
            IEnumerable<string> segments = new[] {_settings.ServiceUri.AbsolutePath.Trim('/'), queueDescription.Path.Trim('/')}
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
                        throw;
                }

                if (!created)
                    queueDescription = await NamespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueDescription.Path,
                    string.Join(", ", new[]
                    {
                        queueDescription.EnableExpress ? "express" : "",
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : "",
                        queueDescription.RequiresSession ? "session" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

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
                        throw;
                }

                if (!created)
                    topicDescription = await RootNamespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Topic: {0} ({1})", topicDescription.Path,
                    string.Join(", ", new[]
                    {
                        topicDescription.EnableExpress ? "express" : "",
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            return topicDescription;
        }

        public async Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription description)
        {
            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await RootNamespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(subscriptionDescription.ForwardTo))
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.WarnFormat("Removing invalid subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                            subscriptionDescription.ForwardTo);
                    }

                    await RootNamespaceManager.DeleteSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                }
                else
                {
                    create = false;
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
                        _log.DebugFormat("Creating subscription {0} -> {1}", description.TopicPath, description.Name);


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
                        throw;
                }

                if (!created)
                    subscriptionDescription = await RootNamespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                    subscriptionDescription.ForwardTo);
            }

            return subscriptionDescription;
        }

        public async Task<SubscriptionDescription> CreateTopicSubscription(string subscriptionName, string topicPath, string queuePath,
            QueueDescription queueDescription)
        {
            var description = Defaults.CreateSubscriptionDescription(topicPath, subscriptionName, queueDescription, queuePath);

            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await RootNamespaceManager.GetSubscriptionAsync(topicPath, subscriptionName).ConfigureAwait(false);
                if (queuePath.Equals(subscriptionDescription.ForwardTo))
                {
                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Updating subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                            subscriptionDescription.ForwardTo);
                    }

                    await RootNamespaceManager.UpdateSubscriptionAsync(description).ConfigureAwait(false);

                    create = false;
                }
                else
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.WarnFormat("Removing invalid subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                            subscriptionDescription.ForwardTo);
                    }

                    await RootNamespaceManager.DeleteSubscriptionAsync(topicPath, subscriptionName).ConfigureAwait(false);
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
                        _log.DebugFormat("Creating subscription {0} -> {1}", topicPath, queuePath);


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
                        throw;
                }

                if (!created)
                    subscriptionDescription = await RootNamespaceManager.GetSubscriptionAsync(topicPath, subscriptionName).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                    subscriptionDescription.ForwardTo);
            }

            return subscriptionDescription;
        }

        Task<MessagingFactory> CreateMessagingFactory()
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                TransportType = _settings.TransportType
            };

            switch (_settings.TransportType)
            {
                case TransportType.NetMessaging:
                    mfs.NetMessagingTransportSettings = _settings.NetMessagingTransportSettings;
                    break;
                case TransportType.Amqp:
                    mfs.AmqpTransportSettings = _settings.AmqpTransportSettings;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return CreateFactory(mfs);
        }

        async Task<MessagingFactory> CreateFactory(MessagingFactorySettings mfs)
        {
            var builder = new UriBuilder(_settings.ServiceUri) {Path = ""};

            var messagingFactory = await MessagingFactory.CreateAsync(builder.Uri, mfs).ConfigureAwait(false);

            messagingFactory.RetryPolicy = new RetryExponential(_settings.RetryMinBackoff, _settings.RetryMaxBackoff, _settings.RetryLimit);

            return messagingFactory;
        }

        Task<MessagingFactory> CreateNetMessagingFactory()
        {
            if (_settings.TransportType == TransportType.NetMessaging)
                return _messagingFactory.Value;

            var mfs = new MessagingFactorySettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                TransportType = TransportType.NetMessaging,
                NetMessagingTransportSettings = _settings.NetMessagingTransportSettings
            };

            return CreateFactory(mfs);
        }

        NamespaceManager CreateNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                RetryPolicy = new RetryExponential(_settings.RetryMinBackoff, _settings.RetryMaxBackoff, _settings.RetryLimit)
            };

            return new NamespaceManager(_settings.ServiceUri, nms);
        }

        NamespaceManager CreateRootNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                RetryPolicy = new RetryExponential(_settings.RetryMinBackoff, _settings.RetryMaxBackoff, _settings.RetryLimit)
            };
            var builder = new UriBuilder(_settings.ServiceUri)
            {
                Path = ""
            };

            return new NamespaceManager(builder.Uri, nms);
        }


        class Handle :
            HostHandle
        {
            readonly Task<MessagingFactory> _messagingFactoryTask;
            readonly Lazy<Task<MessagingFactory>> _sessionFactory;
            readonly ServiceBusHostSettings _settings;
            readonly TaskSupervisor _supervisor;

            public Handle(Task<MessagingFactory> messagingFactoryTask, Lazy<Task<MessagingFactory>> sessionFactory, ServiceBusHostSettings settings,
                TaskSupervisor supervisor)
            {
                _messagingFactoryTask = messagingFactoryTask;
                _sessionFactory = sessionFactory;
                _settings = settings;
                _supervisor = supervisor;
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
                try
                {
                    await _supervisor.Stop("Host stopped", cancellationToken).ConfigureAwait(false);

                    var factory = await _messagingFactoryTask.ConfigureAwait(false);

                    if (!factory.IsClosed)
                        await factory.CloseAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                        _log.Warn("Exception closing messaging factory", ex);
                }

                if (_sessionFactory.IsValueCreated && _settings.TransportType == TransportType.Amqp)
                {
                    try
                    {
                        var factory = await _sessionFactory.Value.ConfigureAwait(false);

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
}