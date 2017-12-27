// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Pipeline.Pipes;
    using Microsoft.ServiceBus.Messaging;
    using Policies;
    using Topology;
    using Transports;
    using Util;


    public class ServiceBusPublishEndpointProvider :
        IPublishEndpointProvider,
        ISendObserverConnector
    {
        readonly ISendEndpointCache _endpointCache;
        readonly IServiceBusHost _host;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly SendObservable _sendObservable;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public ServiceBusPublishEndpointProvider(IServiceBusHost host, ISendEndpointProvider sendEndpointProvider, IPublishPipe publishPipe,
            IServiceBusPublishTopology publishTopology, IMessageSerializer serializer, Uri sourceAddress)
        {
            _host = host;
            _publishPipe = publishPipe;
            _publishTopology = publishTopology;
            _serializer = serializer;
            _sourceAddress = sourceAddress;

            _endpointCache = new SendEndpointCache(sendEndpointProvider);

            _publishObservable = new PublishObservable();
            _sendObservable = new SendObservable();
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message) where T : class
        {
            IServiceBusMessagePublishTopology<T> messageTopology = _publishTopology.GetMessageTopology<T>();

            if (!messageTopology.TryGetPublishAddress(_host.Address, out var publishAddress))
                throw new PublishException($"An address for publishing message type {TypeMetadataCache<T>.ShortName} was not found.");

            return _endpointCache.GetSendEndpoint(publishAddress, x => CreateSendEndpoint(x, messageTopology.TopicDescription));
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        Task<ISendEndpoint> CreateSendEndpoint(Uri address, TopicDescription topicDescription)
        {
            return _host.RetryPolicy.Retry<ISendEndpoint>(async () =>
            {
                var topic = await _host.RootNamespaceManager.CreateTopicSafeAsync(topicDescription).ConfigureAwait(false);

                var messagingFactory = await _host.MessagingFactory.ConfigureAwait(false);

                var topicClient = messagingFactory.CreateTopicClient(topic.Path);

                var client = new TopicSendClient(topicClient);

                var sendTransport = new ServiceBusSendTransport(client, _host.Supervisor);

                sendTransport.ConnectSendObserver(_sendObservable);

                return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress, SendPipe.Empty);
            });
        }


        /// <summary>
        /// This will be used for the topology publish broker layout
        /// </summary>
        class Settings :
            PublishSettings
        {
            public Settings(TopicDescription topicDescription, BrokerTopology brokerTopology)
            {
                TopicDescription = topicDescription;
                BrokerTopology = brokerTopology;
            }

            public TopicDescription TopicDescription { get; }
            public BrokerTopology BrokerTopology { get; }
        }
    }
}