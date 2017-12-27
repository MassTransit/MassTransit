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
namespace MassTransit.AzureServiceBusTransport.Configuration.Specifications
{
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Observers;
    using MassTransit.Topology.Topologies;
    using Topology.Configuration;
    using Topology.Conventions.PartitionKey;
    using Topology.Conventions.SessionId;
    using Topology.Topologies;


    public class ServiceBusTopologyConfiguration :
        IServiceBusTopologyConfiguration
    {
        readonly ServiceBusConsumeTopology _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IServiceBusPublishTopologyConfigurator _publishTopology;
        readonly ConnectHandle _publishToSendTopologyHandle;
        readonly IServiceBusSendTopologyConfigurator _sendTopology;

        public ServiceBusTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new ServiceBusSendTopology();
            _sendTopology.Connect(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));
            _sendTopology.AddConvention(new SessionIdSendTopologyConvention());
            _sendTopology.AddConvention(new PartitionKeySendTopologyConvention());

            _publishTopology = new ServiceBusPublishTopology(messageTopology);
            _publishTopology.Connect(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishToSendTopologyHandle = _publishTopology.Connect(observer);

            _consumeTopology = new ServiceBusConsumeTopology(messageTopology, _publishTopology);
        }

        public ServiceBusTopologyConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new ServiceBusConsumeTopology(topologyConfiguration.Message, topologyConfiguration.Publish);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IServiceBusPublishTopologyConfigurator IServiceBusTopologyConfiguration.Publish => _publishTopology;
        IServiceBusSendTopologyConfigurator IServiceBusTopologyConfiguration.Send => _sendTopology;
        IServiceBusConsumeTopologyConfigurator IServiceBusTopologyConfiguration.Consume => _consumeTopology;

        public void SeparatePublishFromSendTopology()
        {
            _publishToSendTopologyHandle?.Disconnect();
        }
    }
}