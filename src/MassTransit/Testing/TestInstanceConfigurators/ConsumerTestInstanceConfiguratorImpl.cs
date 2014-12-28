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
namespace MassTransit.Testing.TestInstanceConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BuilderConfigurators;
    using Builders;
    using Configurators;
    using ScenarioBuilders;
    using ScenarioConfigurators;
    using TestDecorators;


    public class ConsumerTestInstanceConfiguratorImpl<TScenario, TConsumer> :
        TestInstanceConfiguratorImpl<TScenario>,
        ConsumerTestInstanceConfigurator<TScenario, TConsumer>
        where TConsumer : class, IConsumer
        where TScenario : IBusEndpointTestScenario
    {
        readonly IList<ConsumerTestBuilderConfigurator<TScenario, TConsumer>> _configurators;

        Func<TScenario, ConsumerTestBuilder<TScenario, TConsumer>> _builderFactory;
        IConsumerFactory<TConsumer> _consumerFactory;

        public ConsumerTestInstanceConfiguratorImpl(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
            : base(scenarioBuilderFactory)
        {
            _configurators = new List<ConsumerTestBuilderConfigurator<TScenario, TConsumer>>();

            _builderFactory = scenario => new ConsumerTestBuilderImpl<TScenario, TConsumer>(scenario);
        }

        public void UseBuilder(Func<TScenario, ConsumerTestBuilder<TScenario, TConsumer>> builderFactory)
        {
            _builderFactory = builderFactory;
        }

        public void AddConfigurator(ConsumerTestBuilderConfigurator<TScenario, TConsumer> configurator)
        {
            _configurators.Add(configurator);
        }

        public void UseConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public new IEnumerable<TestConfiguratorResult> Validate()
        {
            if (_consumerFactory == null)
                yield return this.Failure("UseConsumerFactory", "The consumer factory must be configured (using ConstructedBy)");

            IEnumerable<TestConfiguratorResult> results = base.Validate().Concat(_configurators.SelectMany(x => x.Validate()));
            foreach (TestConfiguratorResult result in results)
            {
                yield return result;
            }
        }

        public ConsumerTest<TScenario, TConsumer> Build()
        {
            AddConfigurator(new ConsumerScenarioBuilderConfigurator(_consumerFactory));

            TScenario context = BuildTestScenario();

            ConsumerTestBuilder<TScenario, TConsumer> builder = _builderFactory(context);

            builder.SetConsumerFactory(_consumerFactory);

            builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

            BuildTestActions(builder);

            return builder.Build();
        }


        class ConsumerScenarioBuilderConfigurator :
            IScenarioBuilderConfigurator<TScenario>
        {
            readonly IConsumerFactory<TConsumer> _consumerFactory;
            ReceivedMessageList _received;

            public ConsumerScenarioBuilderConfigurator(IConsumerFactory<TConsumer> consumerFactory, ReceivedMessageList received)
            {
                _received = received;
                var decoratedConsumerFactory = new TestConsumerFactoryDecorator<TConsumer>(consumerFactory, _received);

                _consumerFactory = decoratedConsumerFactory;
            }

            public ITestScenarioBuilder<TScenario> Configure(ITestScenarioBuilder<TScenario> builder)
            {
                var scenarioBuilder = builder as IBusEndpointTestScenarioBuilder;
                if (scenarioBuilder != null)
                    scenarioBuilder.ConfigureReceiveEndpoint(x => x.Consumer(_consumerFactory));

                return builder;
            }

            public IEnumerable<TestConfiguratorResult> Validate()
            {
                yield break;
            }
        }
    }
}