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
namespace MassTransit.Testing
{
    using Decorators;
    using MessageObservers;
    using Saga;


    public class SagaTestHarness<TSaga>
        where TSaga : class, ISaga
    {
        readonly ReceivedMessageList _consumed;
        readonly ISagaRepository<TSaga> _repository;
        readonly SagaList<TSaga> _created;
        readonly SagaList<TSaga> _sagas;

        public SagaTestHarness(BusTestHarness testHarness, ISagaRepository<TSaga> repository)
        {
            _repository = repository;

            _consumed = new ReceivedMessageList(testHarness.TestTimeout);
            _created = new SagaList<TSaga>(testHarness.TestTimeout);
            _sagas = new SagaList<TSaga>(testHarness.TestTimeout);

            testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
        }

        public IReceivedMessageList Consumed => _consumed;
        public ISagaList<TSaga> Sagas => _sagas;
        public ISagaList<TSaga> Created => _sagas;

        void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            var decorator = new SagaRepositoryTestDecorator<TSaga>(_repository, _consumed, _created, _sagas);

            configurator.Saga(decorator);
        }
    }
}