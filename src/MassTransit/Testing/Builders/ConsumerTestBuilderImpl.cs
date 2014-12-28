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
namespace MassTransit.Testing.Builders
{
    using System.Collections.Generic;
    using Instances;
    using Subjects;
    using TestActions;


    public class ConsumerTestBuilderImpl<TScenario, TConsumer> :
        IConsumerTestBuilder<TScenario, TConsumer>
        where TConsumer : class, IConsumer
        where TScenario : IBusTestScenario
    {
        readonly IList<ITestAction<TScenario>> _actions;
        readonly TScenario _scenario;
        IConsumerFactory<TConsumer> _consumerFactory;

        public ConsumerTestBuilderImpl(TScenario scenario)
        {
            _scenario = scenario;

            _actions = new List<ITestAction<TScenario>>();
        }

        public IConsumerTest<TScenario, TConsumer> Build()
        {
            return new ConsumerTest<TScenario, TConsumer>(_scenario, _actions, new ConsumerTestSubject<TScenario, TConsumer>(_consumerFactory));
        }

        public void SetConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public void AddTestAction(ITestAction<TScenario> testAction)
        {
            _actions.Add(testAction);
        }
    }
}