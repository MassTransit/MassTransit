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
namespace MassTransit.AzureServiceBusTransport.Configuration.Builders
{
    using System;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class ServiceBusSubscriptionEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly IServiceBusHost _host;
        readonly ISendTransportProvider _sendTransportProvider;

        public ServiceBusSubscriptionEndpointBuilder(IBusBuilder busBuilder, IServiceBusHost host, IServiceBusEndpointConfiguration configuration,
            ISendTransportProvider sendTransportProvider)
            : base(busBuilder, configuration)
        {
            _configuration = configuration;
            _host = host;
            _sendTransportProvider = sendTransportProvider;
        }

        public IServiceBusReceiveEndpointTopology CreateReceiveEndpointTopology(Uri inputAddress, SubscriptionSettings settings)
        {
            var topologyLayout = BuildTopology(settings);

            return new ServiceBusReceiveEndpointTopology(_configuration, inputAddress, MessageSerializer, _host, _sendTransportProvider, topologyLayout);
        }

        BrokerTopology BuildTopology(SubscriptionSettings settings)
        {
            var topologyBuilder = new SubscriptionEndpointBrokerTopologyBuilder();

            topologyBuilder.Topic = topologyBuilder.CreateTopic(settings.TopicDescription);

            topologyBuilder.CreateSubscription(topologyBuilder.Topic, settings.SubscriptionDescription);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}