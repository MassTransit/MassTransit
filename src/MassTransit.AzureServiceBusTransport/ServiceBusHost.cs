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
        readonly Lazy<Task<NamespaceManager>> _namespaceManager;
        readonly Lazy<Task<NamespaceManager>> _rootNamespaceManager;
        readonly Lazy<Task<MessagingFactory>> _sessionMessagingFactory;
        readonly ServiceBusHostSettings _settings;
        readonly TaskSupervisor _supervisor;

        public ServiceBusHost(ServiceBusHostSettings settings)
        {
            _settings = settings;
            _messagingFactory = new Lazy<Task<MessagingFactory>>(CreateMessagingFactory);
            _sessionMessagingFactory = new Lazy<Task<MessagingFactory>>(CreateNetMessagingFactory);
            _namespaceManager = new Lazy<Task<NamespaceManager>>(CreateNamespaceManager);
            _rootNamespaceManager = new Lazy<Task<NamespaceManager>>(CreateRootNamespaceManager);

            _messageNameFormatter = new ServiceBusMessageNameFormatter();

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<ServiceBusHost>.ShortName} - {_settings.ServiceUri}");
        }

        public HostHandle Start()
        {
            return new Handle(_messagingFactory.Value, _sessionMessagingFactory, _settings, _supervisor);
        }

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

        Task<NamespaceManager> IServiceBusHost.NamespaceManager => _namespaceManager.Value;

        Task<NamespaceManager> IServiceBusHost.RootNamespaceManager => _rootNamespaceManager.Value;

        IMessageNameFormatter IServiceBusHost.MessageNameFormatter => _messageNameFormatter;

        public ITaskSupervisor Supervisor => _supervisor;

        public string GetQueuePath(QueueDescription queueDescription)
        {
            IEnumerable<string> segments = new[] {_settings.ServiceUri.AbsolutePath.Trim('/'), queueDescription.Path.Trim('/')}
                .Where(x => x.Length > 0);

            return string.Join("/", segments);
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

        Task<NamespaceManager> CreateNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                RetryPolicy = new RetryExponential(_settings.RetryMinBackoff, _settings.RetryMaxBackoff, _settings.RetryLimit)
            };

            return Task.FromResult(new NamespaceManager(_settings.ServiceUri, nms));
        }

        Task<NamespaceManager> CreateRootNamespaceManager()
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

            return Task.FromResult(new NamespaceManager(builder.Uri, nms));
        }


        class Handle :
            HostHandle
        {
            readonly Task<MessagingFactory> _messagingFactoryTask;
            readonly Lazy<Task<MessagingFactory>> _sessionFactory;
            readonly ServiceBusHostSettings _settings;
            readonly TaskSupervisor _supervisor;

            public Handle(Task<MessagingFactory> messagingFactoryTask, Lazy<Task<MessagingFactory>> sessionFactory, ServiceBusHostSettings settings, TaskSupervisor supervisor)
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