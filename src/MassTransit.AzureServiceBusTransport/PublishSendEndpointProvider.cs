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
    using Serialization;
    using Transports;


    public class PublishSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly IServiceBusHost[] _hosts;
        readonly ILog _log = Logger.Get<ServiceBusSendEndpointProvider>();
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public PublishSendEndpointProvider(IMessageSerializer serializer, Uri sourceAddress, IServiceBusHost[] hosts)
        {
            _hosts = hosts;
            _sourceAddress = sourceAddress;
            _serializer = serializer;
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            IServiceBusHost host =
                _hosts.FirstOrDefault(x => x.Settings.ServiceUri.Host.Equals(address.Host, StringComparison.OrdinalIgnoreCase));
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            TopicDescription topicDescription = await CreateTopic(await host.RootNamespaceManager, address);

            MessagingFactory messagingFactory = await host.MessagingFactory;

            MessageSender messageSender = await messagingFactory.CreateMessageSenderAsync(topicDescription.Path);

            var sendTransport = new AzureServiceBusSendTransport(messageSender);

            return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress);
        }

        async Task<TopicDescription> CreateTopic(NamespaceManager namespaceManager, Uri address)
        {
            TopicDescription topicDescription = address.GetTopicDescription();
            string topicPath = topicDescription.Path;

            bool create = true;
            try
            {
                topicDescription = await namespaceManager.GetTopicAsync(topicPath);

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
                        _log.DebugFormat("Creating topic {0}", topicPath);

                    topicDescription = await namespaceManager.CreateTopicAsync(topicDescription);
                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                if (!created)
                    topicDescription = await namespaceManager.GetTopicAsync(topicPath);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Topic: {0} ({1})", topicDescription.Path,
                    string.Join(", ", new[]
                    {
                        topicDescription.EnableExpress ? "express" : "",
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            return topicDescription;
        }
    }
}