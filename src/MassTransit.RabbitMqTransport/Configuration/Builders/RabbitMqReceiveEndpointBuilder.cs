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
namespace MassTransit.RabbitMqTransport.Builders
{
    using System.Collections.Generic;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IRabbitMqReceiveEndpointConfiguration _configuration;

        public RabbitMqReceiveEndpointBuilder(IRabbitMqReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_configuration.BindMessageExchanges)
            {
                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Bind();
            }

            return base.ConnectConsumePipe(pipe);
        }

        public RabbitMqReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            IDeadLetterTransport deadLetterTransport = CreateDeadLetterTransport();
            IErrorTransport errorTransport = CreateErrorTransport();

            var receiveEndpointContext = new RabbitMqQueueReceiveEndpointContext(_configuration, brokerTopology);

            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            return receiveEndpointContext;
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new RabbitMqErrorTransport(errorSettings.ExchangeName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new RabbitMqDeadLetterTransport(deadLetterSettings.ExchangeName, filter);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder(settings);
            topologyBuilder.QueueBind();
            topologyBuilder.ExchangeDeclare();

            _configuration.Topology.Consume.Apply(topologyBuilder);
            return topologyBuilder.BuildTopologyLayout();
        }
    }
}
