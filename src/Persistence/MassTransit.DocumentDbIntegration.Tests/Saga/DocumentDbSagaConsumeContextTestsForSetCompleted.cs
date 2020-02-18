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
namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using DocumentDbIntegration.Saga.Context;
    using MassTransit.Saga;
    using Messages;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class DocumentDbSagaConsumeContextTestsForSetCompleted
    {
        [Test]
        public void ThenContextIsSetToComplete()
        {
            Assert.That(_documentDbSagaConsumeContext.IsCompleted, Is.True);
        }

        [Test]
        public async Task ThenSagaDoesNotExistInRepository()
        {
            var saga = await SagaRepository.Instance.GetSaga<SimpleSaga>(_saga.CorrelationId, true);

            Assert.That(saga, Is.Null);
        }

        SimpleSaga _saga;
        DocumentDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga> _documentDbSagaConsumeContext;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaConsumeContext_WhenSettingComplete()
        {
            _saga = new SimpleSaga {CorrelationId = Guid.NewGuid()};

            await SagaRepository.Instance.InsertSaga(_saga, true);

            var databaseContext = new DocumentDbDatabaseContext<SimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName,
                SagaRepository.CollectionName);
            _documentDbSagaConsumeContext =
                new DocumentDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga>(databaseContext, Mock.Of<ConsumeContext<InitiateSimpleSaga>>(), _saga,
                    SagaConsumeContextMode.Insert);

            await _documentDbSagaConsumeContext.SetCompleted();
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_saga.CorrelationId);
        }
    }
}
