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
namespace MassTransit.Testing.TestInstanceConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BuilderConfigurators;
    using Builders;
    using Configurators;
    using ScenarioBuilders;
    using Subjects;


    public class ConsumerTestConfigurator<TScenario, TConsumer> :
        TestConfigurator<TScenario>,
        IConsumerTestConfigurator<TScenario, TConsumer>
        where TConsumer : class, IConsumer
        where TScenario : IBusTestScenario
    {
        readonly IList<IConsumerTestSpecification<TScenario, TConsumer>> _testSpecifications;
        IConsumerFactory<TConsumer> _consumerFactory;

        Func<TScenario, IConsumerTestBuilder<TScenario, TConsumer>> _testBuilderFactory;

        public ConsumerTestConfigurator(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
            : base(scenarioBuilderFactory)
        {
            _testSpecifications = new List<IConsumerTestSpecification<TScenario, TConsumer>>();

            _testBuilderFactory = scenario => new ConsumerTestBuilderImpl<TScenario, TConsumer>(scenario);
        }

        public void UseTestBuilder(Func<TScenario, IConsumerTestBuilder<TScenario, TConsumer>> builderFactory)
        {
            _testBuilderFactory = builderFactory;
        }

        public void AddTestConfigurator(IConsumerTestSpecification<TScenario, TConsumer> configurator)
        {
            _testSpecifications.Add(configurator);
        }

        public void UseConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public new IEnumerable<TestConfiguratorResult> Validate()
        {
            if (_consumerFactory == null)
                yield return this.Failure("UseConsumerFactory", "The consumer factory must be configured (using ConstructedBy)");

            IEnumerable<TestConfiguratorResult> results = base.Validate().Concat(_testSpecifications.SelectMany(x => x.Validate()));
            foreach (var result in results)
            {
                yield return result;
            }
        }

        public IConsumerTest<TScenario, TConsumer> Build()
        {
            var consumerTestSubject = new ConsumerTestSubject<TScenario, TConsumer>(_consumerFactory);

            AddScenarioConfigurator(consumerTestSubject);

            var scenario = BuildTestScenario();

            IConsumerTestBuilder<TScenario, TConsumer> builder = _testBuilderFactory(scenario);

            builder.SetConsumerTestSubject(consumerTestSubject);

            builder = _testSpecifications.Aggregate(builder, (current, configurator) => configurator.Configure(current));

            BuildTestActions(builder);

            return builder.Build();
        }
    }
}