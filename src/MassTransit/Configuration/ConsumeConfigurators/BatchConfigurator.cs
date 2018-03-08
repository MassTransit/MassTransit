// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using ConsumeConnectors;


    public class BatchConfigurator<TMessage> :
        IBatchConfigurator<TMessage>
        where TMessage : class
    {
        readonly IReceiveEndpointConfigurator _configurator;

        public BatchConfigurator(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;

            MessageLimit = 10;
            TimeLimit = TimeSpan.FromSeconds(10);
        }

        public TimeSpan TimeLimit { private get; set; }
        public int MessageLimit { private get; set; }

        void IBatchConfigurator<TMessage>.Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure)
        {
            var specification = ConsumerConnectorCache<TConsumer>.Connector.CreateConsumerSpecification<TConsumer>();
            specification.ConnectConsumerConfigurationObserver(_configurator);

            var messageSpecification = specification.GetMessageSpecification<Batch<TMessage>>();

            configure?.Invoke(messageSpecification);

            var batchSpecification = new BatchConsumerSpecification<TConsumer, TMessage>(messageSpecification, consumerFactory, MessageLimit, TimeLimit);

            _configurator.AddEndpointSpecification(batchSpecification);
        }
    }
}
