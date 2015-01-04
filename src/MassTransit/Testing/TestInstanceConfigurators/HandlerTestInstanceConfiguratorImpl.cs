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
    using System.Threading.Tasks;
    using BuilderConfigurators;
    using Builders;
    using Configurators;
    using ScenarioBuilders;
    using Subjects;


    public class HandlerTestConfigurator<TScenario, TMessage> :
        TestConfigurator<TScenario>,
        IHandlerTestConfigurator<TScenario, TMessage>
        where TMessage : class
        where TScenario : IBusTestScenario
    {
        readonly IList<IHandlerTestSpecification<TScenario, TMessage>> _configurators;

        MessageHandler<TMessage> _handler;

        public HandlerTestConfigurator(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
            : base(scenarioBuilderFactory)
        {
            _configurators = new List<IHandlerTestSpecification<TScenario, TMessage>>();

            _handler = DefaultHandler;
        }

        public void AddTestConfigurator(IHandlerTestSpecification<TScenario, TMessage> configurator)
        {
            _configurators.Add(configurator);
        }

        public void Handler(MessageHandler<TMessage> handler)
        {
            _handler = handler;
        }

        public override IEnumerable<TestConfiguratorResult> Validate()
        {
            if (_handler == null)
                yield return this.Failure("Handler", "Must not be null");


            foreach (TestConfiguratorResult result in base.Validate())
            {
                yield return result;
            }

            foreach (TestConfiguratorResult result in _configurators.SelectMany(x => x.Validate()))
            {
                yield return result;
            }
        }

        public IHandlerTest<TScenario, TMessage> Build()
        {
            var handlerTestSubject = new HandlerTestSubject<TScenario, TMessage>(_handler);

            AddScenarioConfigurator(handlerTestSubject);

            TScenario scenario = BuildTestScenario();

            IHandlerTestBuilder<TScenario, TMessage> builder = new HandlerTestBuilder<TScenario, TMessage>(scenario, handlerTestSubject);

            builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

            BuildTestActions(builder);

            return builder.Build();
        }

        async Task DefaultHandler(ConsumeContext<TMessage> context)
        {
        }
    }
}