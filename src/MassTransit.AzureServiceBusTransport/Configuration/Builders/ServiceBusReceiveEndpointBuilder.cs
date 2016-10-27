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
namespace MassTransit.AzureServiceBusTransport.Builders
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Settings;
    using Transport;
    using Transports;


    public class ServiceBusReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly bool _subscribeMessageTopics;
        readonly IServiceBusHost _host;
        readonly List<TopicSubscriptionSettings> _topicSubscriptions;

        public ServiceBusReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder busBuilder,
            bool subscribeMessageTopics, IServiceBusHost host)
            : base(consumePipe, busBuilder)
        {
            _subscribeMessageTopics = subscribeMessageTopics;
            _host = host;
            _topicSubscriptions = new List<TopicSubscriptionSettings>();
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_subscribeMessageTopics)
                _topicSubscriptions.AddRange(_host.MessageNameFormatter.GetTopicSubscription(typeof(T)));

            return base.ConnectConsumePipe(pipe);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new ServiceBusSendEndpointProvider(MessageSerializer, sourceAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, QueueCacheDurationProvider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var provider = new PublishSendEndpointProvider(MessageSerializer, sourceAddress, _host);

            var cache = new SendEndpointCache(provider, TopicCacheDurationProvider);

            var pipe = CreatePublishPipe(specifications);

            return new ServiceBusPublishEndpointProvider(_host, cache, pipe);
        }

        TimeSpan QueueCacheDurationProvider(Uri address)
        {
            var timeSpan = address.GetQueueDescription().AutoDeleteOnIdle;

            return timeSpan > TimeSpan.FromDays(1) ? TimeSpan.FromDays(1) : timeSpan;
        }

        TimeSpan TopicCacheDurationProvider(Uri address)
        {
            var timeSpan = address.GetTopicDescription().AutoDeleteOnIdle;

            return timeSpan > TimeSpan.FromDays(1) ? TimeSpan.FromDays(1) : timeSpan;
        }

        public IEnumerable<TopicSubscriptionSettings> GetTopicSubscriptions()
        {
            return _topicSubscriptions;
        }
    }
}