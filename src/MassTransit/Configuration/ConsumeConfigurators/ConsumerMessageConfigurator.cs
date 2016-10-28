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
    using GreenPipes;
    using Internals.Extensions;
    using Util;


    public class ConsumerMessageConfigurator<TConsumer, TMessage> :
        IConsumerMessageConfigurator<TConsumer, TMessage>,
        IConsumerMessageConfigurator<TMessage>
        where TConsumer : class, IConsumer
        where TMessage : class
    {
        readonly IPipeConfigurator<ConsumerConsumeContext<TConsumer>> _configurator;

        public ConsumerMessageConfigurator(IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumerConsumeContextSpecificationProxy(specification));
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumeContextSpecificationProxy(specification));
        }


        class ConsumeContextSpecificationProxy :
            IPipeSpecification<ConsumerConsumeContext<TConsumer>>
        {
            readonly IPipeSpecification<ConsumeContext<TMessage>> _specification;

            public ConsumeContextSpecificationProxy(IPipeSpecification<ConsumeContext<TMessage>> specification)
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


        class ConsumerConsumeContextSpecificationProxy :
            IPipeSpecification<ConsumerConsumeContext<TConsumer>>
        {
            readonly IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> _specification;

            public ConsumerConsumeContextSpecificationProxy(IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>>;

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