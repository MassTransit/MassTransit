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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public class ServiceBusConnectionContext :
        ConnectionContext
    {
        readonly CancellationToken _cancellationToken;
        readonly IServiceBusHost _host;
        readonly PayloadCache _payloadCache;

        public ServiceBusConnectionContext(IServiceBusHost host, CancellationToken cancellationToken)
        {
            _host = host;
            _cancellationToken = cancellationToken;
            _payloadCache = new PayloadCache();
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

        public Task<MessagingFactory> MessagingFactory
        {
            get { return _host.MessagingFactory; }
        }

        public Task<NamespaceManager> NamespaceManager
        {
            get { return _host.NamespaceManager; }
        }

        public Task<NamespaceManager> RootNamespaceManager
        {
            get { return _host.RootNamespaceManager; }
        }

        public Uri GetQueueAddress(QueueDescription queueDescription)
        {
            return _host.Settings.GetInputAddress(queueDescription);
        }

        public string GetQueuePath(QueueDescription queueDescription)
        {
            return _host.GetQueuePath(queueDescription);
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationToken; }
        }
    }
}