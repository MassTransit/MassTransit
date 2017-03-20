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
namespace MassTransit.AzureServiceBusTransport.Configurators
{
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using MassTransit.Builders;
    using Microsoft.ServiceBus.Messaging;
    using Pipeline;
    using Settings;
    using Specifications;
    using Transport;
    using Transports;


    public class ServiceBusSubscriptionEndpointSpecification :
        ServiceBusEndpointSpecification,
        IServiceBusSubscriptionEndpointConfigurator
    {
        readonly SubscriptionEndpointSettings _settings;
        readonly IServiceBusEndpointConfiguration _configuration;

        public ServiceBusSubscriptionEndpointSpecification(IServiceBusHost host,BusHostCollection<ServiceBusHost> hosts, string subscriptionName, string topicName,
            IServiceBusEndpointConfiguration configuration)
            : this(host, hosts, new SubscriptionEndpointSettings(topicName, subscriptionName), configuration)
        {
        }

        public ServiceBusSubscriptionEndpointSpecification(IServiceBusHost host, BusHostCollection<ServiceBusHost> hosts, SubscriptionEndpointSettings settings, IServiceBusEndpointConfiguration configuration)
            : base(host, hosts, settings, configuration)
        {
            _settings = settings;
            _configuration = configuration;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public override void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new ServiceBusSubscriptionEndpointBuilder(builder, Host, Hosts, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            ApplyReceiveEndpoint(receivePipe, receiveEndpointTopology, x =>
            {
                x.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointTopology.TopologyLayout, false));
                x.UseFilter(new PrepareSubscriptionClientFilter(_settings));
            });
        }


        protected override ReceiveEndpointSettings GetReceiveEndpointSettings(string queueName)
        {
            var description = new QueueDescription(queueName)
            {
                AutoDeleteOnIdle = _settings.AutoDeleteOnIdle,
                DefaultMessageTimeToLive = _settings.DefaultMessageTimeToLive,
                MaxDeliveryCount = _settings.MaxDeliveryCount,
                RequiresSession = _settings.RequiresSession
            };

            if (_settings.UsingBasicTier == false)
            {
                description.AutoDeleteOnIdle = _settings.AutoDeleteOnIdle;
            }


            return new ReceiveEndpointSettings(description);
        }
    }
}