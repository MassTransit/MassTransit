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


    public class HandlerTestBuilder<TScenario, TMessage> :
        IHandlerTestBuilder<TScenario, TMessage>
        where TMessage : class
        where TScenario : IBusTestScenario
    {
        readonly IList<ITestAction<TScenario>> _actions;
        readonly IHandlerTestSubject<TMessage> _handlerTestSubject;
        readonly TScenario _scenario;

        public HandlerTestBuilder(TScenario scenario, IHandlerTestSubject<TMessage> subject)
        {
            _scenario = scenario;

            _actions = new List<ITestAction<TScenario>>();
            _handlerTestSubject = subject;
        }

        public IHandlerTest<TScenario, TMessage> Build()
        {
            return new HandlerTest<TScenario, TMessage>(_scenario, _actions, _handlerTestSubject);
        }

        public void AddTestAction(ITestAction<TScenario> testAction)
        {
            _actions.Add(testAction);
        }
    }
}