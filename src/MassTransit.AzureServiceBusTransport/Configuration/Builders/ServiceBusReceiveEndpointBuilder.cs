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
    using System.Linq;
    using GreenPipes;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class ServiceBusReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly IServiceBusHost _host;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly bool _subscribeMessageTopics;

        public ServiceBusReceiveEndpointBuilder(IBusBuilder busBuilder, IServiceBusHost host, bool subscribeMessageTopics,
            IServiceBusEndpointConfiguration configuration, ISendTransportProvider sendTransportProvider)
            : base(busBuilder, configuration)
        {
            _subscribeMessageTopics = subscribeMessageTopics;
            _configuration = configuration;
            _sendTransportProvider = sendTransportProvider;
            _host = host;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_subscribeMessageTopics)
            {
                var subscriptionName = "{queuePath}";

                var suffix = _host.Address.AbsolutePath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (!string.IsNullOrWhiteSpace(suffix))
                    subscriptionName += $"-{suffix}";

                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Subscribe(subscriptionName);
            }

            return base.ConnectConsumePipe(pipe);
        }

        public IServiceBusReceiveEndpointTopology CreateReceiveEndpointTopology(Uri inputAddress, ReceiveSettings settings)
        {
            var topologyLayout = BuildTopology(settings);

            return new ServiceBusReceiveEndpointTopology(_configuration, inputAddress, MessageSerializer, _host, _sendTransportProvider, topologyLayout);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            topologyBuilder.Queue = topologyBuilder.CreateQueue(settings.QueueDescription);

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}