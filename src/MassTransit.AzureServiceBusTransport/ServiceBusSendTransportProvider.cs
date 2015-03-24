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
    using Logging;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class ServiceBusSendTransportProvider :
        ISendTransportProvider
    {
        readonly IServiceBusHost[] _hosts;
        readonly ILog _log = Logger.Get<ServiceBusSendEndpointProvider>();

        public ServiceBusSendTransportProvider(IServiceBusHost[] hosts)
        {
            _hosts = hosts;
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            IServiceBusHost host = _hosts.FirstOrDefault(
                x => address.ToString().StartsWith(x.Settings.ServiceUri.ToString(), StringComparison.OrdinalIgnoreCase));
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            QueueDescription queueDescription = await CreateQueue(await host.NamespaceManager, address);

            MessagingFactory messagingFactory = await host.MessagingFactory;

            string queuePath = host.GetQueuePath(queueDescription);

            MessageSender messageSender = await messagingFactory.CreateMessageSenderAsync(queuePath);

            var sendTransport = new ServiceBusSendTransport(messageSender);
            return sendTransport;
        }

        async Task<QueueDescription> CreateQueue(NamespaceManager namespaceManager, Uri address)
        {
            QueueDescription queueDescription = address.GetQueueDescription();
            bool create = true;
            try
            {
                queueDescription = await namespaceManager.GetQueueAsync(queueDescription.Path);

                create = false;
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                bool created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating queue {0}", queueDescription.Path);

                    queueDescription = await namespaceManager.CreateQueueAsync(queueDescription);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    // seems a conflict occurs rather than an already exists exception
                    if (mex.Detail.ErrorCode == 409)
                    {
                    }
                    else
                        throw;
                }

                if (!created)
                    queueDescription = await namespaceManager.GetQueueAsync(queueDescription.Path);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueDescription.Path,
                    string.Join(", ", new[]
                    {
                        queueDescription.EnableExpress ? "express" : "",
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            return queueDescription;
        }
    }
}