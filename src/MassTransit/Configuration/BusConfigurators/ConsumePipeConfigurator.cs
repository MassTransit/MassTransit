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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using Observables;
    using Pipeline;
    using Saga;
    using Saga.SubscriptionConfigurators;


    public class ConsumePipeConfigurator :
        IConsumePipeConfigurator,
        IConsumePipeFactory,
        IConsumePipeSpecification
    {
        readonly ConsumerConfigurationObservable _consumerObservers;
        readonly SagaConfigurationObservable _sagaObservers;
        readonly IList<IConsumePipeSpecification> _specifications;

        public ConsumePipeConfigurator()
        {
            _specifications = new List<IConsumePipeSpecification>();
            _consumerObservers = new ConsumerConfigurationObservable();
            _sagaObservers = new SagaConfigurationObservable();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy(specification));
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification) where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy<T>(specification));
        }

        public ConnectHandle ConnectConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _consumerObservers.Connect(observer);
        }

        public void ConfigureConsumer<TConsumer>(IConsumerConfigurator<TConsumer> configurator) where TConsumer : class
        {
            _consumerObservers.ConfigureConsumer(configurator);
        }

        public void ConfigureConsumerMessage<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator) where TConsumer : class
            where TMessage : class
        {
            _consumerObservers.ConfigureConsumerMessage(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator) where TSaga : class, ISaga
        {
            _sagaObservers.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator) where TSaga : class, ISaga
            where TMessage : class
        {
            _sagaObservers.SagaMessageConfigured(configurator);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _sagaObservers.Connect(observer);
        }

        public IConsumePipe CreateConsumePipe(params IConsumePipeSpecification[] specifications)
        {
            var builder = new ConsumePipeBuilder();

            Apply(builder);

            for (var i = 0; i < specifications.Length; i++)
                specifications[i].Apply(builder);

            return builder.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(IConsumePipeBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }


        class Proxy :
            IConsumePipeSpecification
        {
            readonly IPipeSpecification<ConsumeContext> _specification;

            public Proxy(IPipeSpecification<ConsumeContext> specification)
            {
                _specification = specification;
            }

            public void Apply(IConsumePipeBuilder builder)
            {
                _specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }


        class Proxy<T> :
            IConsumePipeSpecification
            where T : class
        {
            readonly IPipeSpecification<ConsumeContext<T>> _specification;

            public Proxy(IPipeSpecification<ConsumeContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(IConsumePipeBuilder builder)
            {
                _specification.Apply(new BuilderProxy(builder));
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }


            class BuilderProxy :
                IPipeBuilder<ConsumeContext<T>>
            {
                readonly IConsumePipeBuilder _builder;

                public BuilderProxy(IConsumePipeBuilder builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<ConsumeContext<T>> filter)
                {
                    _builder.AddFilter(filter);
                }
            }
        }
    }
}