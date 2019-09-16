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
namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Configuration.Configuration;
    using Context;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class SqsQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        SqsReceiveEndpointContext
    {
        readonly IAmazonSqsReceiveEndpointConfiguration _configuration;
        readonly IAmazonSqsPublishTopology _publishTopology;

        public SqsQueueReceiveEndpointContext(IAmazonSqsReceiveEndpointConfiguration configuration, BrokerTopology brokerTopology)
            : base(configuration)
        {
            _configuration = configuration;
            BrokerTopology = brokerTopology;

            _publishTopology = configuration.Topology.Publish;
        }

        public BrokerTopology BrokerTopology { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new SendTransportProvider(_configuration);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new PublishTransportProvider(_configuration.HostConfiguration);
        }
    }
}
