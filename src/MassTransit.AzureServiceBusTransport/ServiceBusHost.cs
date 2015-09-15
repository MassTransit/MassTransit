// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;
    using Transports;


    public class ServiceBusHost :
        IServiceBusHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<ServiceBusHost>();
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly Lazy<Task<MessagingFactory>> _messagingFactory;
        readonly Lazy<Task<NamespaceManager>> _namespaceManager;
        readonly Lazy<Task<NamespaceManager>> _rootNamespaceManager;
        readonly ServiceBusHostSettings _settings;

        public ServiceBusHost(ServiceBusHostSettings settings)
        {
            _settings = settings;
            _messagingFactory = new Lazy<Task<MessagingFactory>>(CreateMessagingFactory);
            _namespaceManager = new Lazy<Task<NamespaceManager>>(CreateNamespaceManager);
            _rootNamespaceManager = new Lazy<Task<NamespaceManager>>(CreateRootNamespaceManager);

            _messageNameFormatter = new ServiceBusMessageNameFormatter();
        }

        public HostHandle Start()
        {
            return new Handle(_messagingFactory.Value);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("host");
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

        public string GetQueuePath(QueueDescription queueDescription)
        {
            return string.Join("/", _settings.ServiceUri.AbsolutePath.Trim('/'), queueDescription.Path);
        }

        Task<MessagingFactory> CreateMessagingFactory()
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                TransportType = TransportType.Amqp,
                AmqpTransportSettings = new AmqpTransportSettings
                {
                    BatchFlushInterval = TimeSpan.FromMilliseconds(50)
                }
            };

            var builder = new UriBuilder(_settings.ServiceUri) {Path = ""};

            return MessagingFactory.CreateAsync(builder.Uri, mfs);
        }

        Task<NamespaceManager> CreateNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = TimeSpan.FromSeconds(10),
                RetryPolicy = RetryPolicy.NoRetry
            };

            return Task.FromResult(new NamespaceManager(_settings.ServiceUri, nms));
        }

        Task<NamespaceManager> CreateRootNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = TimeSpan.FromSeconds(10),
                RetryPolicy = RetryPolicy.NoRetry
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

            public Handle(Task<MessagingFactory> messagingFactoryTask)
            {
                _messagingFactoryTask = messagingFactoryTask;
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
                try
                {
                    MessagingFactory factory = await _messagingFactoryTask.ConfigureAwait(false);

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