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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class ServiceBusSubscriptionEndpointConfigurator :
        ServiceBusEndpointConfigurator,
        IServiceBusSubscriptionEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IServiceBusHost _host;
        readonly SubscriptionEndpointSettings _settings;
        bool _subscribeMessageTopics;

        public ServiceBusSubscriptionEndpointConfigurator(IServiceBusHost host, string subscriptionName, string topicName, IConsumePipe consumePipe = null)
            : this(host, new SubscriptionEndpointSettings(topicName, subscriptionName), consumePipe)
        {
        }

        public ServiceBusSubscriptionEndpointConfigurator(IServiceBusHost host, SubscriptionEndpointSettings settings, IConsumePipe consumePipe = null)
            : base(host, settings, consumePipe)
        {
            _host = host;
            _settings = settings;
            _subscribeMessageTopics = true;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_settings.PrefetchCount <= 0)
                yield return this.Failure("PrefetchCount", "must be > 0");
            if (_settings.MaxConcurrentCalls <= 0)
                yield return this.Failure("MaxConcurrentCalls", "must be > 0");
            if (_settings.SubscriptionDescription.AutoDeleteOnIdle != TimeSpan.Zero
                && _settings.SubscriptionDescription.AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public void Apply(IBusBuilder builder)
        {
            ServiceBusReceiveEndpointBuilder endpointBuilder = null;
            var receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new ServiceBusReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter, _subscribeMessageTopics);
                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            var transport = new ReceiveTransport(_host, _settings);

            builder.AddReceiveEndpoint(_settings.Path, new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetInputAddress()
        {
            return _host.Settings.GetInputAddress(_settings.SubscriptionDescription);
        }

        protected override Uri GetErrorAddress()
        {
            var errorQueueName = _settings.SubscriptionDescription.Name + "_error";

            var errorQueueDescription = GetQueueDescription(errorQueueName);

            return _host.Settings.GetInputAddress(errorQueueDescription);
        }

        QueueDescription GetQueueDescription(string errorQueueName)
        {
            return new QueueDescription(errorQueueName)
            {
                AutoDeleteOnIdle = _settings.SubscriptionDescription.AutoDeleteOnIdle,
                DefaultMessageTimeToLive = _settings.SubscriptionDescription.DefaultMessageTimeToLive,
                MaxDeliveryCount = _settings.SubscriptionDescription.MaxDeliveryCount,
                RequiresSession = _settings.SubscriptionDescription.RequiresSession
            };
        }

        protected override Uri GetDeadLetterAddress()
        {
            var skippedQueueName = _settings.SubscriptionDescription.Name + "_skipped";

            var errorQueueDescription = GetQueueDescription(skippedQueueName);

            return _host.Settings.GetInputAddress(errorQueueDescription);
        }
    }
}