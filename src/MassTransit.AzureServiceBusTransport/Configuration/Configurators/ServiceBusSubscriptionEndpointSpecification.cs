// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Pipeline;
    using Settings;


    public class ServiceBusSubscriptionEndpointSpecification :
        ServiceBusEndpointSpecification,
        IServiceBusSubscriptionEndpointConfigurator
    {
        readonly SubscriptionEndpointSettings _settings;

        public ServiceBusSubscriptionEndpointSpecification(IServiceBusHost host, string subscriptionName, string topicName, IConsumePipe consumePipe = null)
            : this(host, new SubscriptionEndpointSettings(topicName, subscriptionName), consumePipe)
        {
        }

        public ServiceBusSubscriptionEndpointSpecification(IServiceBusHost host, SubscriptionEndpointSettings settings, IConsumePipe consumePipe = null)
            : base(host, settings, consumePipe)
        {
            _settings = settings;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public override void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new ServiceBusReceiveEndpointBuilder(CreateConsumePipe(builder), builder, false, Host);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            ApplyReceiveEndpoint(receiveEndpointBuilder, receivePipe,
                new PrepareSubscriptionEndpointFilter(_settings),
                new PrepareSubscriptionClientFilter(_settings));
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