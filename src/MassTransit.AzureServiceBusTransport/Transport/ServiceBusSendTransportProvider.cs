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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Policies;
    using Transports;


    public class ServiceBusSendTransportProvider :
        ISendTransportProvider
    {
        readonly IServiceBusHost[] _hosts;

        public ServiceBusSendTransportProvider(IServiceBusHost[] hosts)
        {
            _hosts = hosts;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            IServiceBusHost host;
            if (!TryGetMatchingHost(address, out host))
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            return host.RetryPolicy.Retry<ISendTransport>(async () =>
            {
                var queueDescription = address.GetQueueDescription();

                string queuePath;
                var namespacePath = host.NamespaceManager.Address.AbsolutePath.Trim('/');

                if (string.IsNullOrEmpty(namespacePath))
                {
                    queueDescription = await host.CreateQueue(queueDescription).ConfigureAwait(false);

                    queuePath = host.GetQueuePath(queueDescription);
                }
                else if (IsInNamespace(queueDescription, namespacePath))
                {
                    queueDescription.Path = queueDescription.Path.Replace(namespacePath, "").Trim('/');
                    queueDescription = await host.CreateQueue(queueDescription).ConfigureAwait(false);

                    queuePath = host.GetQueuePath(queueDescription);
                }
                else
                {
//                    queueDescription = await host.CreateQueue(queueDescription).ConfigureAwait(false);
                    queueDescription = await host.RootNamespaceManager.CreateQueueSafeAsync(queueDescription).ConfigureAwait(false);

                    queuePath = queueDescription.Path;
                }

                var messagingFactory = await host.MessagingFactory.ConfigureAwait(false);

                var queueClient = messagingFactory.CreateQueueClient(queuePath);

                var client = new QueueSendClient(queueClient);

                return new ServiceBusSendTransport(client, host.Supervisor);
            });
        }

        bool TryGetMatchingHost(Uri address, out IServiceBusHost host)
        {
            host = _hosts
                .Where(
                    x =>
                        x.Settings.ServiceUri.GetLeftPart(UriPartial.Authority).Equals(address.GetLeftPart(UriPartial.Authority),
                            StringComparison.OrdinalIgnoreCase))
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