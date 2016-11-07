// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using MassTransit.Testing.Instances;
    using MassTransit.Testing.Subjects;
    using MassTransit.Testing.TestActions;


    public class StateMachineSagaTestInstance<TScenario, TSaga, TStateMachine> :
        TestInstance<TScenario>,
        SagaTest<TScenario, TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : ITestScenario
        where TStateMachine : SagaStateMachine<TSaga>
    {
        readonly StateMachineSagaTestSubjectImpl<TScenario, TSaga, TStateMachine> _subject;
        bool _disposed;

        public StateMachineSagaTestInstance(TScenario scenario, IList<ITestAction<TScenario>> actions,
            ISagaRepository<TSaga> sagaRepository, TStateMachine stateMachine)
            : base(scenario, actions)
        {
            _subject = new StateMachineSagaTestSubjectImpl<TScenario, TSaga, TStateMachine>(sagaRepository, stateMachine);
        }

        SagaTestSubject<TSaga> SagaTest<TSaga>.Saga => _subject;

        public override async Task DisposeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (_disposed)
                return;

            await _subject.DisposeAsync(cancellationToken).ConfigureAwait(false);

            await base.DisposeAsync(cancellationToken).ConfigureAwait(false);

            _disposed = true;
        }
    }
}