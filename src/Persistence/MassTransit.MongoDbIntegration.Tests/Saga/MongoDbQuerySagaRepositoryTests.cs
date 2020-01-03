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
namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MongoDbIntegration.Saga;
    using NUnit.Framework;


    [TestFixture]
    public class MongoDbQuerySagaRepositoryTests
    {
        [Test]
        public void ThenCorrelationIdsReturned()
        {
            Assert.That(_result.Single(), Is.EqualTo(_correlationId));
        }

        Guid _correlationId;
        IEnumerable<Guid> _result;

        [OneTimeSetUp]
        public async Task GivenAMongoDbQuerySagaRepository_WhenFindingSaga()
        {
            _correlationId = Guid.NewGuid();

            await SagaRepository.InsertSaga(new SimpleSaga {CorrelationId = _correlationId});

            var repository = new MongoDbQuerySagaRepository<SimpleSaga>(SagaRepository.Instance);

            ISagaQuery<SimpleSaga> query = new SagaQuery<SimpleSaga>(x => x.CorrelationId == _correlationId);

            _result = await repository.Find(query);
        }

        [OneTimeTearDown]
        public Task Kill()
        {
            return SagaRepository.DeleteSaga(_correlationId);
        }
    }
}
