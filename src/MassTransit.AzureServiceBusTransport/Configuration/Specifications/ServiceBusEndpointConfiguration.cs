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
namespace MassTransit.AzureServiceBusTransport.Specifications
{
    using EndpointSpecifications;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using Topology;


    public class ServiceBusEndpointConfiguration :
        EndpointConfiguration<IServiceBusEndpointConfiguration, IServiceBusConsumeTopologyConfigurator, IServiceBusSendTopologyConfigurator, IServiceBusPublishTopologyConfigurator>,
        IServiceBusEndpointConfiguration
    {
        public ServiceBusEndpointConfiguration()
            : this(CreateConsume(), CreateSend(GlobalTopology.Send), CreatePublish(GlobalTopology.Publish))
        {
        }

        public ServiceBusEndpointConfiguration(IServiceBusConsumeTopologyConfigurator consumeTopology, IServiceBusSendTopologyConfigurator sendTopology,
            IServiceBusPublishTopologyConfigurator publishTopology)
            : base(consumeTopology, sendTopology, publishTopology)
        {
        }

        public ServiceBusEndpointConfiguration(IServiceBusConsumeTopologyConfigurator consumeTopology, IServiceBusSendTopologyConfigurator sendTopology,
            IServiceBusPublishTopologyConfigurator publishTopology,
            IServiceBusEndpointConfiguration configuration, IConsumePipe consumePipe = null)
            : base(consumeTopology, sendTopology, publishTopology, configuration, consumePipe)
        {
        }

        static readonly ServiceBusMessageNameFormatter _messageNameFormatter;
        static readonly MessageNameFormatterEntityNameFormatter _entityNameFormatter;

        static ServiceBusEndpointConfiguration()
        {
            _messageNameFormatter = new ServiceBusMessageNameFormatter();
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_messageNameFormatter);
        }

        static IServiceBusConsumeTopologyConfigurator CreateConsume()
        {
            return new ServiceBusConsumeTopology(_entityNameFormatter);
        }

        static IServiceBusSendTopologyConfigurator CreateSend(ISendTopology delegateSendTopology)
        {
            var sendTopology = new ServiceBusSendTopology();

            var observer = new DelegateSendTopologyConfigurationObserver(delegateSendTopology);
            sendTopology.Connect(observer);

            return sendTopology;
        }

        static IServiceBusPublishTopologyConfigurator CreatePublish(IPublishTopology delegatePublishTopology)
        {
            var publishTopology = new ServiceBusPublishTopology(_entityNameFormatter);

            var observer = new DelegatePublishTopologyConfigurationObserver(delegatePublishTopology);
            publishTopology.Connect(observer);

            return publishTopology;
        }

        public override IServiceBusEndpointConfiguration CreateConfiguration(IConsumePipe consumePipe = null)
        {
            return new ServiceBusEndpointConfiguration(CreateConsume(), CreateSend(SendTopology), CreatePublish(PublishTopology), this, consumePipe);
        }
    }
}