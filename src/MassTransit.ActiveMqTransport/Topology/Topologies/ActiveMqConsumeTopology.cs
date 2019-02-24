// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Builders;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Specifications;
    using Util;


    public class ActiveMqConsumeTopology :
        ConsumeTopology,
        IActiveMqConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IActiveMqPublishTopology _publishTopology;
        readonly IList<IActiveMqConsumeTopologySpecification> _specifications;

        public ActiveMqConsumeTopology(IMessageTopology messageTopology, IActiveMqPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _specifications = new List<IActiveMqConsumeTopologySpecification>();
        }

        IActiveMqMessageConsumeTopology<T> IActiveMqConsumeTopology.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IActiveMqMessageConsumeTopologyConfigurator<T>;
        }

        public void AddSpecification(IActiveMqConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IActiveMqMessageConsumeTopologyConfigurator<T> IActiveMqConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IActiveMqMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IActiveMqMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string topicName, Action<ITopicBindingConfigurator> configure = null)
        {
            if (topicName.StartsWith("VirtualTopic."))
            {
                var consumerName = $"Consumer.{{queue}}{topicName}";

                var specification = new ConsumerConsumeTopologySpecification(topicName, consumerName);

                configure?.Invoke(specification);

                _specifications.Add(specification);
            }
            else
                _specifications.Add(new InvalidActiveMqConsumeTopologySpecification("Bind", $"Only virtual topics can be bound: {topicName}"));
        }

        public override string CreateTemporaryQueueName(string tag)
        {
            var result = base.CreateTemporaryQueueName(tag);
            return new string(result.Where(c => c != '.').ToArray());
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
