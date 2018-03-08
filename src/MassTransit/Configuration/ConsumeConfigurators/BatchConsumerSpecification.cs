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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;
    using Pipeline.ConsumerFactories;
    using Pipeline.Filters;


    public class BatchConsumerSpecification<TConsumer, TMessage> :
        IReceiveEndpointSpecification
        where TMessage : class
        where TConsumer : class, IConsumer<Batch<TMessage>>
    {
        readonly IConsumerMessageSpecification<TConsumer, Batch<TMessage>> _messageSpecification;
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly int _messageLimit;
        readonly TimeSpan _timeLimit;

        public BatchConsumerSpecification(IConsumerMessageSpecification<TConsumer, Batch<TMessage>> messageSpecification,
            IConsumerFactory<TConsumer> consumerFactory, int messageLimit, TimeSpan timeLimit)
        {
            _messageSpecification = messageSpecification;
            _consumerFactory = consumerFactory;
            _messageLimit = messageLimit;
            _timeLimit = timeLimit;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate().Concat(_messageSpecification.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            var filter = new MethodConsumerMessageFilter<TConsumer, Batch<TMessage>>();

            var consumerPipe = _messageSpecification.Build(filter);

            var batchConsumerFactory = new BatchConsumerFactory<TConsumer, TMessage>(_consumerFactory, _messageLimit, _timeLimit, consumerPipe);

            var specification = ConsumerConnectorCache<IConsumer<TMessage>>.Connector.CreateConsumerSpecification<IConsumer<TMessage>>();

            ConsumerConnectorCache<IConsumer<TMessage>>.Connector.ConnectConsumer(builder, batchConsumerFactory, specification);
        }
    }
}