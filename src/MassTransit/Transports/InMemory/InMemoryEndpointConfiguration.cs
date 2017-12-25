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
namespace MassTransit.Transports.InMemory
{
    using EndpointSpecifications;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;
    using Pipeline;
    using Topology;
    using Topology.Configurators;
    using Topology.Topologies;


    public class InMemoryEndpointConfiguration :
        EndpointConfiguration<IInMemoryEndpointConfiguration, IInMemoryConsumeTopologyConfigurator, ISendTopologyConfigurator, IInMemoryPublishTopologyConfigurator>,
        IInMemoryEndpointConfiguration
    {
        public InMemoryEndpointConfiguration(IMessageTopologyConfigurator messageTopology)
            : this(messageTopology, CreateConsume(messageTopology), CreateSend(GlobalTopology.Send), CreatePublish(messageTopology, GlobalTopology.Publish))
        {
        }

        InMemoryEndpointConfiguration(IMessageTopologyConfigurator messageTopology, IInMemoryConsumeTopologyConfigurator consumeTopology, ISendTopologyConfigurator sendTopology,
            IInMemoryPublishTopologyConfigurator publishTopology)
            : base(messageTopology, consumeTopology, sendTopology, publishTopology)
        {
        }

        public InMemoryEndpointConfiguration(IMessageTopologyConfigurator messageTopology, IInMemoryConsumeTopologyConfigurator consumeTopology,
            ISendTopologyConfigurator sendTopology, IInMemoryPublishTopologyConfigurator publishTopology, IInMemoryEndpointConfiguration configuration,
            IConsumePipe consumePipe = null)
            : base(messageTopology, consumeTopology, sendTopology, publishTopology, configuration, consumePipe)
        {
        }

        public override IInMemoryEndpointConfiguration CreateConfiguration(IConsumePipe consumePipe = null)
        {
            return new InMemoryEndpointConfiguration(MessageTopology, CreateConsume(MessageTopology), CreateSend(SendTopology), CreatePublish(MessageTopology,
                PublishTopology), this, consumePipe);
        }

        static IInMemoryConsumeTopologyConfigurator CreateConsume(IMessageTopologyConfigurator messageTopology)
        {
            var consumeTopology = new InMemoryConsumeTopology(messageTopology);

            return consumeTopology;
        }

        static ISendTopologyConfigurator CreateSend(ISendTopologyConfigurator delegateSendTopology)
        {
            var sendTopology = new SendTopology();

            var observer = new DelegateSendTopologyConfigurationObserver(delegateSendTopology);
            sendTopology.Connect(observer);

            return sendTopology;
        }

        static IInMemoryPublishTopologyConfigurator CreatePublish(IMessageTopologyConfigurator messageTopology, IPublishTopologyConfigurator delegatePublishTopology)
        {
            var publishTopology = new InMemoryPublishTopology(messageTopology);

            var observer = new DelegatePublishTopologyConfigurationObserver(delegatePublishTopology);
            publishTopology.Connect(observer);

            return publishTopology;
        }
    }
}