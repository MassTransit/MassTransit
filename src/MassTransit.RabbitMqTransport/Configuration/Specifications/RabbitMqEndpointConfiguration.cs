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
namespace MassTransit.RabbitMqTransport.Specifications
{
    using EndpointSpecifications;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;
    using Topology;
    using Topology.Configuration;
    using Topology.Conventions.RoutingKey;
    using Topology.Topologies;


    public class RabbitMqEndpointConfiguration :
        EndpointConfiguration
            <IRabbitMqEndpointConfiguration, IRabbitMqConsumeTopologyConfigurator, IRabbitMqSendTopologyConfigurator, IRabbitMqPublishTopologyConfigurator>,
        IRabbitMqEndpointConfiguration
    {
        public RabbitMqEndpointConfiguration(IMessageTopologyConfigurator messageTopology)
            : this(messageTopology, CreateSend(GlobalTopology.Send), CreatePublish(messageTopology, GlobalTopology.Publish))
        {
        }

        public RabbitMqEndpointConfiguration(IMessageTopologyConfigurator messageTopology, IRabbitMqSendTopologyConfigurator sendTopology,
            IRabbitMqPublishTopologyConfigurator publishTopology)
            : base(messageTopology, CreateConsume(messageTopology, publishTopology), sendTopology, publishTopology)
        {
        }

        public RabbitMqEndpointConfiguration(IMessageTopologyConfigurator messageTopology, IRabbitMqSendTopologyConfigurator sendTopology,
            IRabbitMqPublishTopologyConfigurator publishTopology, IRabbitMqEndpointConfiguration configuration, IConsumePipe consumePipe = null)
            : base(messageTopology, CreateConsume(messageTopology, publishTopology), sendTopology, publishTopology, configuration, consumePipe)
        {
        }

        public override IRabbitMqEndpointConfiguration CreateConfiguration(IConsumePipe consumePipe = null)
        {
            return new RabbitMqEndpointConfiguration(MessageTopology, SendTopology, PublishTopology, this, consumePipe);
        }

        static IRabbitMqConsumeTopologyConfigurator CreateConsume(IMessageTopology messageTopology, IRabbitMqPublishTopology publishTopology)
        {
            var consumeTopology = new RabbitMqConsumeTopology(messageTopology, publishTopology);

            return consumeTopology;
        }

        static IRabbitMqSendTopologyConfigurator CreateSend(ISendTopology delegateSendTopology)
        {
            var sendTopology = new RabbitMqSendTopology(RabbitMqEntityNameValidator.Validator);
            sendTopology.AddConvention(new RoutingKeySendTopologyConvention());

            var observer = new DelegateSendTopologyConfigurationObserver(delegateSendTopology);
            sendTopology.Connect(observer);

            return sendTopology;
        }

        static IRabbitMqPublishTopologyConfigurator CreatePublish(IMessageTopology messageTopology, IPublishTopologyConfigurator delegatePublishTopology)
        {
            var publishTopology = new RabbitMqPublishTopology(messageTopology);

            var observer = new DelegatePublishTopologyConfigurationObserver(delegatePublishTopology);
            publishTopology.Connect(observer);

            return publishTopology;
        }
    }
}