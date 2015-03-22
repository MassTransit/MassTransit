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
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;
    using Transports;


    public class ServiceBusHost :
        IServiceBusHost
    {
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

        ServiceBusHostSettings IServiceBusHost.Settings
        {
            get { return _settings; }
        }

        Task<MessagingFactory> IServiceBusHost.MessagingFactory
        {
            get { return _messagingFactory.Value; }
        }

        Task<NamespaceManager> IServiceBusHost.NamespaceManager
        {
            get { return _namespaceManager.Value; }
        }

        public Task<NamespaceManager> RootNamespaceManager
        {
            get { return _rootNamespaceManager.Value; }
        }

        public IMessageNameFormatter MessageNameFormatter
        {
            get { return _messageNameFormatter; }
        }

        public string GetQueuePath(QueueDescription queueDescription)
        {
            return string.Join("/", _settings.ServiceUri.AbsolutePath.Trim(new[] {'/'}), queueDescription.Path);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            if (_messagingFactory.IsValueCreated)
            {
                MessagingFactory factory = await _messagingFactory.Value;
                if (!factory.IsClosed)
                    await factory.CloseAsync();
            }
        }

        async Task<MessagingFactory> CreateMessagingFactory()
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
                TransportType = TransportType.Amqp,
                AmqpTransportSettings = new AmqpTransportSettings
                {
                    BatchFlushInterval = TimeSpan.FromMilliseconds(50),
                },
            };

            var builder = new UriBuilder(_settings.ServiceUri) {Path = ""};

            return await MessagingFactory.CreateAsync(builder.Uri, mfs);
        }

        async Task<NamespaceManager> CreateNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = TimeSpan.FromSeconds(10),
                RetryPolicy = RetryPolicy.NoRetry,
            };

            return new NamespaceManager(_settings.ServiceUri, nms);
        }

        async Task<NamespaceManager> CreateRootNamespaceManager()
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = TimeSpan.FromSeconds(10),
                RetryPolicy = RetryPolicy.NoRetry,
            };
            var builder = new UriBuilder(_settings.ServiceUri)
            {
                Path = ""
            };

            return new NamespaceManager(builder.Uri, nms);
        }
    }
}