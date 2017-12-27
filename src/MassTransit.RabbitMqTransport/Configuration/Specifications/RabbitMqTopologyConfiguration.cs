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
    using GreenPipes;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Observers;
    using MassTransit.Topology.Topologies;
    using Topology;
    using Topology.Configuration;
    using Topology.Conventions.RoutingKey;
    using Topology.Topologies;


    public class RabbitMqTopologyConfiguration :
        IRabbitMqTopologyConfiguration
    {
        readonly RabbitMqConsumeTopology _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IRabbitMqPublishTopologyConfigurator _publishTopology;
        readonly ConnectHandle _publishToSendTopologyHandle;
        readonly IRabbitMqSendTopologyConfigurator _sendTopology;

        public RabbitMqTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new RabbitMqSendTopology(RabbitMqEntityNameValidator.Validator);
            _sendTopology.Connect(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));
            _sendTopology.AddConvention(new RoutingKeySendTopologyConvention());

            _publishTopology = new RabbitMqPublishTopology(messageTopology);
            _publishTopology.Connect(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishToSendTopologyHandle = _publishTopology.Connect(observer);

            _consumeTopology = new RabbitMqConsumeTopology(messageTopology, _publishTopology);
        }

        public RabbitMqTopologyConfiguration(IRabbitMqTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new RabbitMqConsumeTopology(topologyConfiguration.Message, topologyConfiguration.Publish);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IRabbitMqPublishTopologyConfigurator IRabbitMqTopologyConfiguration.Publish => _publishTopology;
        IRabbitMqSendTopologyConfigurator IRabbitMqTopologyConfiguration.Send => _sendTopology;
        IRabbitMqConsumeTopologyConfigurator IRabbitMqTopologyConfiguration.Consume => _consumeTopology;

        public void SeparatePublishFromSendTopology()
        {
            _publishToSendTopologyHandle?.Disconnect();
        }
    }
}