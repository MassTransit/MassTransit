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
    using Pipeline;
    using Topology;


    public class InMemoryEndpointConfiguration :
        EndpointConfiguration<IInMemoryEndpointConfiguration, IInMemoryConsumeTopologyConfigurator, ISendTopologyConfigurator, IInMemoryPublishTopologyConfigurator>,
        IInMemoryEndpointConfiguration
    {
        public InMemoryEndpointConfiguration()
            : this(CreateConsume(), CreateSend(GlobalTopology.Send), CreatePublish(GlobalTopology.Publish))
        {
        }

        InMemoryEndpointConfiguration(IInMemoryConsumeTopologyConfigurator consumeTopology, ISendTopologyConfigurator sendTopology,
            IInMemoryPublishTopologyConfigurator publishTopology)
            : base(consumeTopology, sendTopology, publishTopology)
        {
        }

        public InMemoryEndpointConfiguration(IInMemoryConsumeTopologyConfigurator consumeTopology, ISendTopologyConfigurator sendTopology,
            IInMemoryPublishTopologyConfigurator publishTopology, IInMemoryEndpointConfiguration configuration, IConsumePipe consumePipe = null)
            : base(consumeTopology, sendTopology, publishTopology, configuration, consumePipe)
        {
        }

        static IInMemoryConsumeTopologyConfigurator CreateConsume()
        {
            var entityNameFormatter = new MessageUrnEntityNameFormatter();
            var consumeTopology = new InMemoryConsumeTopology(entityNameFormatter);

            return consumeTopology;
        }

        static ISendTopologyConfigurator CreateSend(ISendTopology delegateSendTopology)
        {
            var sendTopology = new SendTopology();

            var observer = new DelegateSendTopologyConfigurationObserver(delegateSendTopology);
            sendTopology.Connect(observer);

            return sendTopology;
        }

        static IInMemoryPublishTopologyConfigurator CreatePublish(IPublishTopology delegatePublishTopology)
        {
            var entityNameFormatter = new MessageUrnEntityNameFormatter();
            var publishTopology = new InMemoryPublishTopology(entityNameFormatter);

            var observer = new DelegatePublishTopologyConfigurationObserver(delegatePublishTopology);
            publishTopology.Connect(observer);

            return publishTopology;
        }

        public override IInMemoryEndpointConfiguration CreateConfiguration(IConsumePipe consumePipe = null)
        {
            return new InMemoryEndpointConfiguration(CreateConsume(), CreateSend(SendTopology), CreatePublish(PublishTopology), this, consumePipe);
        }
    }
}