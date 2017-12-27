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
namespace MassTransit.RabbitMqTransport.Builders
{
    using System;
    using GreenPipes;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IRabbitMqReceiveEndpointBuilder
    {
        readonly bool _bindMessageExchanges;
        readonly IRabbitMqHost _host;
        readonly BusHostCollection<RabbitMqHost> _hosts;
        readonly IRabbitMqEndpointConfiguration _configuration;

        public RabbitMqReceiveEndpointBuilder(IBusBuilder busBuilder, IRabbitMqHost host, BusHostCollection<RabbitMqHost> hosts, bool bindMessageExchanges,
            IRabbitMqEndpointConfiguration configuration)
            : base(busBuilder, configuration)
        {
            _bindMessageExchanges = bindMessageExchanges;
            _configuration = configuration;
            _host = host;
            _hosts = hosts;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_bindMessageExchanges)
            {
                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Bind();
            }

            return base.ConnectConsumePipe(pipe);
        }

        public IRabbitMqReceiveEndpointTopology CreateReceiveEndpointTopology(Uri inputAddress, ReceiveSettings settings)
        {
            var topologyLayout = BuildTopology(settings);

            return new RabbitMqReceiveEndpointTopology(_configuration, inputAddress, MessageSerializer, _host, _hosts, topologyLayout);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            topologyBuilder.Queue = topologyBuilder.QueueDeclare(settings.QueueName, settings.Durable, settings.AutoDelete, settings.Exclusive, settings.QueueArguments);

            topologyBuilder.Exchange = topologyBuilder.ExchangeDeclare(settings.ExchangeName ?? settings.QueueName, settings.ExchangeType, settings.Durable, settings.AutoDelete,
                settings.ExchangeArguments);

            topologyBuilder.QueueBind(topologyBuilder.Exchange, topologyBuilder.Queue, settings.RoutingKey, settings.BindingArguments);

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildTopologyLayout();
        }
    }
}