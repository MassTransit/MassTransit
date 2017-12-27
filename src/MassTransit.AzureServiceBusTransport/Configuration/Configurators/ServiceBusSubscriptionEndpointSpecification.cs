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
namespace MassTransit.AzureServiceBusTransport.Configuration.Configurators
{
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using MassTransit.Builders;
    using Pipeline;
    using Settings;
    using Specifications;
    using Transport;


    public class ServiceBusSubscriptionEndpointSpecification :
        ServiceBusEndpointSpecification,
        IServiceBusSubscriptionEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly SubscriptionEndpointSettings _settings;

        public ServiceBusSubscriptionEndpointSpecification(IServiceBusHost host, string subscriptionName, string topicName,
            IServiceBusEndpointConfiguration configuration, ISendTransportProvider sendTransportProvider)
            : this(host, new SubscriptionEndpointSettings(topicName, subscriptionName), configuration, sendTransportProvider)
        {
        }

        public ServiceBusSubscriptionEndpointSpecification(IServiceBusHost host, SubscriptionEndpointSettings settings,
            IServiceBusEndpointConfiguration configuration, ISendTransportProvider sendTransportProvider)
            : base(host, settings, settings.SubscriptionConfigurator, configuration)
        {
            _settings = settings;
            _configuration = configuration;
            _sendTransportProvider = sendTransportProvider;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public override void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new ServiceBusSubscriptionEndpointBuilder(builder, Host, _configuration, _sendTransportProvider);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            ApplyReceiveEndpoint(receivePipe, receiveEndpointTopology, x =>
            {
                x.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointTopology.BrokerTopology, false));
                x.UseFilter(new PrepareSubscriptionClientFilter(_settings));
            });
        }
    }
}