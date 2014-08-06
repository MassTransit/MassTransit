// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public class ServiceBusConnectionContext :
        ConnectionContext,
        IDisposable
    {
        readonly ServiceBusHostSettings _hostSettings;
        readonly object _lock = new object();
        readonly PayloadCache _payloadCache;
        readonly CancellationTokenSource _tokenSource;
        MessagingFactory _messagingFactory;
        NamespaceManager _namespaceManager;
        CancellationTokenRegistration _registration;

        public ServiceBusConnectionContext(ServiceBusHostSettings hostSettings, CancellationToken cancellationToken)
        {
            _hostSettings = hostSettings;
            _payloadCache = new PayloadCache();

            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            _messagingFactory = hostSettings.GetMessagingFactory();
            _namespaceManager = hostSettings.GetNamespaceManager();
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        public MessagingFactory Factory
        {
            get
            {
                lock (_lock)
                {
                    if (_messagingFactory == null)
                        throw new InvalidOperationException("The connection was closed");

                    return _messagingFactory;
                }
            }
        }

        public NamespaceManager NamespaceManager
        {
            get
            {
                lock (_lock)
                {
                    if (_namespaceManager == null)
                        throw new InvalidOperationException("The connection was closed");

                    return _namespaceManager;
                }
            }
        }

        public Uri GetQueueAddress(string queueName)
        {
            return new Uri(_hostSettings.ServiceUri, queueName);
        }

        public CancellationToken CancellationToken
        {
            get { return _tokenSource.Token; }
        }

        public void Dispose()
        {
            Close();
        }

        void OnCancellationRequested()
        {
            _tokenSource.Cancel();

            var factory = _messagingFactory;
            if(factory != null && !factory.IsClosed)
                factory.Close();
        }

        void Close()
        {
            lock (_lock)
            {
                _registration.Dispose();

                _messagingFactory.Close();

                _namespaceManager = null;
                _messagingFactory = null;
            }
        }
    }
}