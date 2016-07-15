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
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class ServiceBusSendTransportProvider :
        ISendTransportProvider
    {
        readonly IServiceBusHost[] _hosts;

        public ServiceBusSendTransportProvider(IServiceBusHost[] hosts)
        {
            _hosts = hosts;
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            IServiceBusHost host;
            if(!TryGetMatchingHost(address, out host))
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            var queueDescription = address.GetQueueDescription();

            var namespaceManager = await host.NamespaceManager.ConfigureAwait(false);

            string queuePath;
            var namespacePath = namespaceManager.Address.AbsolutePath.Trim('/');

            if (string.IsNullOrEmpty(namespacePath))
            {
                queueDescription = await namespaceManager.CreateQueueSafeAsync(queueDescription).ConfigureAwait(false);

                queuePath = host.GetQueuePath(queueDescription);
            }
            else if (IsInNamespace(queueDescription, namespacePath))
            {
                queueDescription.Path = queueDescription.Path.Replace(namespacePath, "").Trim('/');
                queueDescription = await namespaceManager.CreateQueueSafeAsync(queueDescription).ConfigureAwait(false);

                queuePath = host.GetQueuePath(queueDescription);
            }
            else
            {
                namespaceManager = await host.RootNamespaceManager.ConfigureAwait(false);

                queueDescription = await namespaceManager.CreateQueueSafeAsync(queueDescription).ConfigureAwait(false);

                queuePath = queueDescription.Path;
            }

            MessagingFactory messagingFactory = await host.MessagingFactory.ConfigureAwait(false);

            MessageSender messageSender = await messagingFactory.CreateMessageSenderAsync(queuePath).ConfigureAwait(false);

            return new ServiceBusSendTransport(messageSender, host.Supervisor);
        }

        bool TryGetMatchingHost(Uri address, out IServiceBusHost host)
        {
            host = _hosts
                .Where(x => x.Settings.ServiceUri.GetLeftPart(UriPartial.Authority).Equals(address.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => address.AbsolutePath.StartsWith(x.Settings.ServiceUri.AbsolutePath, StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                .FirstOrDefault();

            return host != null;
        }

        static bool IsInNamespace(QueueDescription queueDescription, string namespacePath)
        {
            return queueDescription.Path.StartsWith(namespacePath);
        }
    }
}