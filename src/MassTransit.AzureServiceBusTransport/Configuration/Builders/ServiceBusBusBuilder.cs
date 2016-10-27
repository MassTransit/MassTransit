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
    using BusConfigurators;
    using Configurators;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Settings;
    using Transport;
    using Transports;


    public class ServiceBusBusBuilder :
        BusBuilder
    {
        readonly ServiceBusReceiveEndpointSpecification _busEndpointSpecification;
        readonly BusHostCollection<ServiceBusHost> _hosts;

        public ServiceBusBusBuilder(BusHostCollection<ServiceBusHost> hosts, IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory, ReceiveEndpointSettings settings)
            : base(consumePipeFactory, sendPipeFactory, publishPipeFactory, hosts)
        {
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            _hosts = hosts;

            _busEndpointSpecification = new ServiceBusReceiveEndpointSpecification(_hosts[0], settings, ConsumePipe);

            foreach (var host in hosts.Hosts)
            {
                host.ReceiveEndpointFactory = new ServiceBusReceiveEndpointFactory(this, host);
                host.SubscriptionEndpointFactory = new ServiceBusSubscriptionEndpointFactory(this, host);
            }
        }

        public override IPublishEndpointProvider PublishEndpointProvider => _busEndpointSpecification.PublishEndpointProvider;

        public override ISendEndpointProvider SendEndpointProvider => _busEndpointSpecification.SendEndpointProvider;

        protected override Uri GetInputAddress()
        {
            return _busEndpointSpecification.InputAddress;
        }

        protected override IConsumePipe GetConsumePipe()
        {
            return CreateConsumePipe();
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ServiceBusSendTransportProvider(_hosts);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new ServiceBusSendEndpointProvider(MessageSerializer, sourceAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, QueueCacheDurationProvider);
        }

        TimeSpan QueueCacheDurationProvider(Uri address)
        {
            var timeSpan = address.GetQueueDescription().AutoDeleteOnIdle;

            return timeSpan > TimeSpan.FromDays(1) ? TimeSpan.FromDays(1) : timeSpan;
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var provider = new PublishSendEndpointProvider(MessageSerializer, sourceAddress, _hosts[0]);

            var cache = new SendEndpointCache(provider, TopicCacheDurationProvider);

            var pipe = CreatePublishPipe(specifications);

            return new ServiceBusPublishEndpointProvider(_hosts[0], cache, pipe);
        }

        TimeSpan TopicCacheDurationProvider(Uri address)
        {
            var timeSpan = address.GetTopicDescription().AutoDeleteOnIdle;

            return timeSpan > TimeSpan.FromDays(1) ? TimeSpan.FromDays(1) : timeSpan;
        }

        protected override void PreBuild()
        {
            _busEndpointSpecification.Apply(this);
        }
    }
}