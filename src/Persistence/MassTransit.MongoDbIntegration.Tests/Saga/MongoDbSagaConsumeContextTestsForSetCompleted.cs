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
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MongoDbIntegration.Saga.Context;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class MongoDbSagaConsumeContextTestsForSetCompleted
    {
        [Test]
        public void ThenContextIsSetToComplete()
        {
            Assert.That(_mongoDbSagaConsumeContext.IsCompleted, Is.True);
        }

        [Test]
        public async Task ThenSagaDoesNotExistInRepository()
        {
            var saga = await SagaRepository.GetSaga(_saga.CorrelationId);

            Assert.That(saga, Is.Null);
        }

        SimpleSaga _saga;
        MongoDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga> _mongoDbSagaConsumeContext;

        [OneTimeSetUp]
        public async Task GivenAMongoDbSagaConsumeContext_WhenSettingComplete()
        {
            _saga = new SimpleSaga {CorrelationId = Guid.NewGuid()};

            await SagaRepository.InsertSaga(_saga);

            _mongoDbSagaConsumeContext =
                new MongoDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga>(SagaRepository.Instance.GetCollection<SimpleSaga>("sagas"),
                    Mock.Of<ConsumeContext<InitiateSimpleSaga>>(), _saga, SagaConsumeContextMode.Insert);

            await _mongoDbSagaConsumeContext.SetCompleted();
        }

        [OneTimeTearDown]
        public Task Kill()
        {
            return SagaRepository.DeleteSaga(_saga.CorrelationId);
        }
    }
}
