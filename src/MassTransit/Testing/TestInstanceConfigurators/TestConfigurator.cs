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
    using ActionConfigurators;
    using Builders;
    using Configurators;
    using ScenarioBuilders;
    using ScenarioConfigurators;


    public abstract class TestConfigurator<TScenario>
        where TScenario : ITestScenario
    {
        readonly IList<ITestActionConfigurator<TScenario>> _testActionConfigurators;
        readonly IList<IScenarioSpecification<TScenario>> _scenarioConfigurators;
        Func<ITestScenarioBuilder<TScenario>> _scenarioBuilderFactory;

        protected TestConfigurator(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
        {
            _scenarioConfigurators = new List<IScenarioSpecification<TScenario>>();
            _testActionConfigurators = new List<ITestActionConfigurator<TScenario>>();

            _scenarioBuilderFactory = scenarioBuilderFactory;
        }

        public void AddActionConfigurator(ITestActionConfigurator<TScenario> action)
        {
            _testActionConfigurators.Add(action);
        }

        public void UseScenarioBuilder(Func<ITestScenarioBuilder<TScenario>> contextBuilderFactory)
        {
            _scenarioBuilderFactory = contextBuilderFactory;
        }

        public void AddScenarioConfigurator(IScenarioSpecification<TScenario> configurator)
        {
            _scenarioConfigurators.Add(configurator);
        }

        public virtual IEnumerable<TestConfiguratorResult> Validate()
        {
            return _scenarioConfigurators.SelectMany(x => x.Validate())
                .Concat(_testActionConfigurators.SelectMany(x => x.Validate()));
        }

        protected TScenario BuildTestScenario()
        {
            ITestScenarioBuilder<TScenario> builder = _scenarioBuilderFactory();

            builder = _scenarioConfigurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

            TScenario scenario = builder.Build();

            return scenario;
        }

        protected void BuildTestActions(ITestBuilder<TScenario> builder)
        {
            foreach (var configurator in _testActionConfigurators)
            {
                configurator.Configure(builder);
            }
        }
    }
}