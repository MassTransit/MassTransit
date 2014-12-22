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
namespace Automatonymous.Testing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using MassTransit.Testing.Scenarios;
    using MassTransit.Testing.Subjects;
    using MassTransit.Testing.TestDecorators;
    using RepositoryConfigurators;


    public class StateMachineSagaTestSubjectImpl<TScenario, TSaga, TStateMachine> :
        SagaTestSubject<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : TestScenario
        where TStateMachine : StateMachine<TSaga>
    {
        readonly Action<StateMachineSagaRepositoryConfigurator<TSaga>> _configureCorrelation;
        readonly SagaListImpl<TSaga> _created;
        readonly ReceivedMessageListImpl _received;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly SagaListImpl<TSaga> _sagas;
        readonly TStateMachine _stateMachine;
        bool _disposed;
        ConnectHandle _handle;

        public StateMachineSagaTestSubjectImpl(ISagaRepository<TSaga> sagaRepository, TStateMachine stateMachine,
            Action<StateMachineSagaRepositoryConfigurator<TSaga>> configureCorrelation)
        {
            _sagaRepository = sagaRepository;
            _stateMachine = stateMachine;
            _configureCorrelation = configureCorrelation;

            _received = new ReceivedMessageListImpl();
            _created = new SagaListImpl<TSaga>();
            _sagas = new SagaListImpl<TSaga>();
        }

        public ReceivedMessageList Received
        {
            get { return _received; }
        }

        public SagaList<TSaga> Created
        {
            get { return _created; }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_handle != null)
            {
                _handle.Disconnect();
                _handle = null;
            }

            _received.Dispose();

            _disposed = true;
        }

        public IEnumerator<SagaInstance<TSaga>> GetEnumerator()
        {
            return _sagas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Any()
        {
            return _sagas.Any();
        }

        public bool Any(Func<TSaga, bool> filter)
        {
            return _sagas.Any(filter);
        }

        public TSaga Contains(Guid sagaId)
        {
            return _sagas.Contains(sagaId);
        }

        public void Prepare(TScenario scenario)
        {
            var decoratedSagaRepository = new SagaRepositoryTestDecorator<TSaga>(_sagaRepository, _received, _created,
                _sagas);

            _handle = scenario.InputBus.ConnectStateMachineSaga(_stateMachine, decoratedSagaRepository, _configureCorrelation);
        }
    }
}