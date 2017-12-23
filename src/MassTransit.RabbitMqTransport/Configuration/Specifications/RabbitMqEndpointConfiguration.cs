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
    using Topology;


    public class RabbitMqEndpointConfiguration :
        EndpointConfiguration<IRabbitMqEndpointConfiguration, IRabbitMqConsumeTopologyConfigurator, IRabbitMqSendTopologyConfigurator, IRabbitMqPublishTopologyConfigurator>,
        IRabbitMqEndpointConfiguration
    {
        public RabbitMqEndpointConfiguration(IMessageTopology messageTopology)
            : base(messageTopology, CreateConsume(messageTopology), CreateSend(GlobalTopology.Send), CreatePublish(messageTopology, GlobalTopology.Publish))
        {
        }

        public RabbitMqEndpointConfiguration(IMessageTopology messageTopology, IRabbitMqConsumeTopologyConfigurator consumeTopology, IRabbitMqSendTopologyConfigurator sendTopology,
            IRabbitMqPublishTopologyConfigurator publishTopology)
            : base(messageTopology, consumeTopology, sendTopology, publishTopology)
        {
        }

        public RabbitMqEndpointConfiguration(IMessageTopology messageTopology, IRabbitMqConsumeTopologyConfigurator consumeTopology, IRabbitMqSendTopologyConfigurator sendTopology,
            IRabbitMqPublishTopologyConfigurator publishTopology, IRabbitMqEndpointConfiguration configuration, IConsumePipe consumePipe = null)
            : base(messageTopology, consumeTopology, sendTopology, publishTopology, configuration, consumePipe)
        {
        }

        static IRabbitMqConsumeTopologyConfigurator CreateConsume(IMessageTopology messageTopology)
        {
            var consumeTopology = new RabbitMqConsumeTopology(messageTopology);

            return consumeTopology;
        }

        static IRabbitMqSendTopologyConfigurator CreateSend(ISendTopology delegateSendTopology)
        {
            var sendTopology = new RabbitMqSendTopology();

            var observer = new DelegateSendTopologyConfigurationObserver(delegateSendTopology);
            sendTopology.Connect(observer);

            return sendTopology;
        }

        static IRabbitMqPublishTopologyConfigurator CreatePublish(IMessageTopology messageTopology, IPublishTopology delegatePublishTopology)
        {
            var publishTopology = new RabbitMqPublishTopology(messageTopology);

            var observer = new DelegatePublishTopologyConfigurationObserver(delegatePublishTopology);
            publishTopology.Connect(observer);

            return publishTopology;
        }

        public override IRabbitMqEndpointConfiguration CreateConfiguration(IConsumePipe consumePipe = null)
        {
            return new RabbitMqEndpointConfiguration(MessageTopology, CreateConsume(MessageTopology), CreateSend(SendTopology), CreatePublish(MessageTopology, PublishTopology),
                this, consumePipe);
        }
    }
}