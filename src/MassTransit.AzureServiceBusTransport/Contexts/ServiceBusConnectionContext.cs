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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public class ServiceBusConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        readonly IServiceBusHost _host;

        public ServiceBusConnectionContext(IServiceBusHost host, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _host = host;
        }

        public Task<MessagingFactory> MessagingFactory => _host.MessagingFactory;

        public Task<MessagingFactory> SessionMessagingFactory => _host.SessionMessagingFactory;

        public Task<NamespaceManager> NamespaceManager => _host.NamespaceManager;

        public Task<NamespaceManager> RootNamespaceManager => _host.RootNamespaceManager;

        public Uri GetQueueAddress(QueueDescription queueDescription)
        {
            return _host.Settings.GetInputAddress(queueDescription);
        }

        public Uri GetTopicAddress(Type messageType)
        {
            return _host.MessageNameFormatter.GetTopicAddress(_host, messageType);
        }

        public string GetQueuePath(QueueDescription queueDescription)
        {
            return _host.GetQueuePath(queueDescription);
        }
    }
}