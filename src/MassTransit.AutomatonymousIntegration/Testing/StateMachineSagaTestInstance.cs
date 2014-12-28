// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Testing
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using MassTransit.Testing.Instances;
    using MassTransit.Testing.Scenarios;
    using MassTransit.Testing.Subjects;
    using MassTransit.Testing.TestActions;
    using RepositoryConfigurators;


    public class StateMachineSagaTestInstance<TScenario, TSaga, TStateMachine> :
        TestInstance<TScenario>,
        SagaTest<TScenario, TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : ITestScenario
        where TStateMachine : StateMachine<TSaga>
    {
        readonly StateMachineSagaTestSubjectImpl<TScenario, TSaga, TStateMachine> _subject;

        bool _disposed;

        public StateMachineSagaTestInstance(TScenario scenario, IList<ITestAction<TScenario>> actions,
            ISagaRepository<TSaga> sagaRepository, TStateMachine stateMachine,
            Action<StateMachineSagaRepositoryConfigurator<TSaga>> configureCallback)
            : base(scenario, actions)
        {
            _subject = new StateMachineSagaTestSubjectImpl<TScenario, TSaga, TStateMachine>(sagaRepository,
                stateMachine, configureCallback);
        }

        public SagaTestSubject<TSaga> Saga
        {
            get { return _subject; }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _subject.Dispose();

            base.Dispose(disposing);

            _disposed = true;
        }

        ~StateMachineSagaTestInstance()
        {
            Dispose(false);
        }
    }
}