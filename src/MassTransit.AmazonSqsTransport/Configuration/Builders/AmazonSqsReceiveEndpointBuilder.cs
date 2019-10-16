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
namespace MassTransit.AmazonSqsTransport.Configuration.Builders
{
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class AmazonSqsReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IAmazonSqsReceiveEndpointConfiguration _configuration;

        public AmazonSqsReceiveEndpointBuilder(IAmazonSqsReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_configuration.SubscribeMessageTopics)
            {
                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Subscribe();
            }

            return base.ConnectConsumePipe(pipe);
        }

        public SqsReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            IDeadLetterTransport deadLetterTransport = CreateDeadLetterTransport();
            IErrorTransport errorTransport = CreateErrorTransport();

            var receiveEndpointContext = new SqsQueueReceiveEndpointContext(_configuration, brokerTopology);

            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            return receiveEndpointContext;
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var builder = new ReceiveEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(settings.EntityName, settings.Durable, settings.AutoDelete, settings.QueueAttributes, settings.QueueSubscriptionAttributes, settings.Tags);

            _configuration.Topology.Consume.Apply(builder);

            return builder.BuildTopologyLayout();
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new SqsErrorTransport(errorSettings.EntityName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new SqsDeadLetterTransport(deadLetterSettings.EntityName, filter);
        }
    }
}
