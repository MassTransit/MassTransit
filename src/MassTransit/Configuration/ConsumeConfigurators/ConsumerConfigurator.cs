// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using PipeConfigurators;
    using Pipeline;
    using Policies;


    public class ConsumerConfigurator<TConsumer> :
        IConsumerConfigurator<TConsumer>,
        IReceiveEndpointSpecification
        where TConsumer : class, IConsumer
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        readonly List<IPipeSpecification<ConsumerConsumeContext<TConsumer>>> _pipeBuilderConfigurators;
        readonly IRetryPolicy _retryPolicy;

        public ConsumerConfigurator(IConsumerFactory<TConsumer> consumerFactory, IRetryPolicy retryPolicy)
        {
            _consumerFactory = consumerFactory;
            _retryPolicy = retryPolicy;

            _pipeBuilderConfigurators = new List<IPipeSpecification<ConsumerConsumeContext<TConsumer>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            _pipeBuilderConfigurators.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate().Concat(_pipeBuilderConfigurators.SelectMany(x => x.Validate()));
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            builder.ConnectConsumer(_consumerFactory, _retryPolicy, _pipeBuilderConfigurators.ToArray());
        }
    }
}