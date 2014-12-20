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
namespace MassTransit.TestFramework.Fixtures
{
    using System;
    using Magnum.Reflection;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class SagaTestFixture<TSaga> :
        InMemoryTestFixture
        where TSaga : class, ISaga
    {
        InMemorySagaRepository<TSaga> _repository;

        public InMemorySagaRepository<TSaga> Repository
        {
            get { return _repository; }
        }

        public SagaTestFixture()
        {
            SagaId = NewId.NextGuid();
        }

        [TestFixtureSetUp]
        public void SagaTestFixtureSetup()
        {
            _repository = new InMemorySagaRepository<TSaga>();
        }

        protected Guid SagaId { get; private set; }

        protected TSaga Saga
        {
            get
            {
                Guid? sagaId = _repository.ShouldContainSaga(SagaId);
                if (sagaId.HasValue)
                    return _repository[sagaId.Value];

                return null;
            }
        }

        protected TSaga AddExistingSaga(Guid sagaId, Action<TSaga> initializer)
        {
            TSaga saga = FastActivator<TSaga>.Create(sagaId);

            initializer(saga);

            _repository.Add(saga);

            return saga;
        }
    }
}