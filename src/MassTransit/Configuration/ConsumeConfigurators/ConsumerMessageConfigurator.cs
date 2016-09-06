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
namespace MassTransit.ConsumeConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using GreenPipes;
    using Internals.Extensions;
    using PipeConfigurators;
    using Util;


    public class ConsumerMessageConfigurator<TConsumer, TMessage> :
        IConsumerMessageConfigurator<TMessage>
        where TConsumer : class, IConsumer
        where TMessage : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;

        public ConsumerMessageConfigurator(IConsumerConfigurator<TConsumer> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _configurator.AddPipeSpecification(new SpecificationProxy(specification));
        }


        class SpecificationProxy :
            IPipeSpecification<ConsumerConsumeContext<TConsumer>>
        {
            readonly IPipeSpecification<ConsumeContext<TMessage>> _specification;

            public SpecificationProxy(IPipeSpecification<ConsumeContext<TMessage>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<ConsumeContext<TMessage>>;

                if (messageBuilder != null)
                    _specification.Apply(messageBuilder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                if (!typeof(TConsumer).HasInterface<IConsumer<TMessage>>())
                    yield return this.Failure("MessageType", $"is not consumed by {TypeMetadataCache<TConsumer>.ShortName}");

                foreach (var validationResult in _specification.Validate())
                {
                    yield return validationResult;
                }
            }
        }
    }
}