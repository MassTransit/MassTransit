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
namespace MassTransit.Testing.Subjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Configurators;
    using Decorators;
    using GreenPipes.Util;
    using MessageObservers;
    using Saga;
    using ScenarioBuilders;
    using ScenarioConfigurators;


    public class SagaTestSubjectImpl<TScenario, TSaga> :
        SagaTestSubject<TSaga>,
        IScenarioSpecification<TScenario>
        where TSaga : class, ISaga
        where TScenario : IBusTestScenario
    {
        readonly ISagaRepository<TSaga> _sagaRepository;
        SagaList<TSaga> _created;
        ReceivedMessageList _received;
        SagaList<TSaga> _sagas;

        public SagaTestSubjectImpl(ISagaRepository<TSaga> sagaRepository)
        {
            _sagaRepository = sagaRepository;
        }

        public ITestScenarioBuilder<TScenario> Configure(ITestScenarioBuilder<TScenario> builder)
        {
            _received = new ReceivedMessageList(builder.Timeout);
            _created = new SagaList<TSaga>(builder.Timeout);
            _sagas = new SagaList<TSaga>(builder.Timeout);

            var decoratedSagaRepository = new SagaRepositoryTestDecorator<TSaga>(_sagaRepository, _received, _created, _sagas);

            var scenarioBuilder = builder as IBusTestScenarioBuilder;
            scenarioBuilder?.ConfigureReceiveEndpoint(x => x.Saga(decoratedSagaRepository));

            return builder;
        }

        public IEnumerable<TestConfiguratorResult> Validate()
        {
            yield break;
        }

        public IEnumerable<ISagaInstance<TSaga>> Select()
        {
            return _created.Select();
        }

        public IEnumerable<ISagaInstance<TSaga>> Select(Func<TSaga, bool> filter)
        {
            return _created.Select(filter);
        }

        public TSaga Contains(Guid sagaId)
        {
            return _created.Contains(sagaId);
        }

        public IReceivedMessageList Received => _received;

        public ISagaList<TSaga> Created => _created;

        public IEnumerator<ISagaInstance<TSaga>> GetEnumerator()
        {
            return _sagas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Task DisposeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return TaskUtil.Completed;
        }
    }
}