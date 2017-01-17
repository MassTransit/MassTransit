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
namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using MassTransit.Topology;
    using Newtonsoft.Json.Linq;
    using Util;


    public class ServiceBusMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IServiceBusMessageConsumeTopologyConfigurator<TMessage>,
        IServiceBusMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IList<IServiceBusConsumeTopologySpecification> _specifications;

        public ServiceBusMessageConsumeTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
            : base(entityNameFormatter)
        {
            _specifications = new List<IServiceBusConsumeTopologySpecification>();
        }

        bool IsBindableMessageType => typeof(JToken) != typeof(TMessage);

        IServiceBusMessageConsumeTopologyConfigurator<T> IServiceBusMessageConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            var result = this as IServiceBusMessageConsumeTopologyConfigurator<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public void Apply(IReceiveEndpointConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
            {
                specification.Apply(builder);
            }
        }

        public void Subscribe(string subscriptionName, Action<ISubscriptionConfigurator> configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidServiceBusConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var topicPath = EntityNameFormatter.FormatEntityName();

            var temporary = TypeMetadataCache<TMessage>.IsTemporaryMessageType;

            var topicConfigurator = new TopicConfigurator(topicPath);
            if (temporary)
            {
                topicConfigurator.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                topicConfigurator.EnableExpress = true;
            }

            var subscriptionConfigurator = new SubscriptionConfigurator(topicPath, subscriptionName);

            configure?.Invoke(subscriptionConfigurator);

            var specification = new SubscriptionConsumeTopologySpecification(topicConfigurator.GetTopicDescription(),
                subscriptionConfigurator.GetSubscriptionDescription());

            _specifications.Add(specification);
        }
    }
}