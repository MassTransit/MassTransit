// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class ServiceBusReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IServiceBusReceiveEndpointConfiguration _configuration;

        public ServiceBusReceiveEndpointBuilder(IServiceBusReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_configuration.SubscribeMessageTopics)
            {
                var subscriptionName = GenerateSubscriptionName();

                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Subscribe(subscriptionName);
            }

            return base.ConnectConsumePipe(pipe);
        }

        public ServiceBusReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var topologyLayout = BuildTopology(_configuration.Settings);

            return new ServiceBusEntityReceiveEndpointContext(_configuration, topologyLayout, ReceiveObservers, TransportObservers, EndpointObservers);
        }

        string GenerateSubscriptionName()
        {
            var subscriptionName = "{queuePath}";

            var suffix = _configuration.HostAddress.AbsolutePath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (!string.IsNullOrWhiteSpace(suffix))
                subscriptionName += $"-{suffix}";

            return subscriptionName;
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            topologyBuilder.Queue = topologyBuilder.CreateQueue(settings.GetQueueDescription());

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}