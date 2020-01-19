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
    using GreenPipes;
    using MassTransit.Saga;
    using MongoDbIntegration.Saga;
    using MongoDbIntegration.Saga.Context;
    using MongoDbIntegration.Saga.Pipeline;
    using MongoDB.Driver;
    using Moq;
    using NUnit.Framework;
    using Pipeline;


    [TestFixture]
    public class MongoDbSagaRepositoryTestsForSendQuery
    {
        [Test]
        public void ThenMissingPipeNotCalled()
        {
            _sagaPolicy.Verify(x => x.Missing(_sagaQueryConsumeContext.Object, It.IsAny<MissingPipe<SimpleSaga, InitiateSimpleSaga>>()), Times.Never);
        }

        [Test]
        public void ThenSagaSentToInstance()
        {
            _sagaPolicy.Verify(x => x.Existing(_sagaConsumeContext.Object, _nextPipe.Object));
        }

        [Test]
        public async Task ThenVersionIncremented()
        {
            var saga = await SagaRepository.GetSaga(_correlationId);

            Assert.That(saga.Version, Is.EqualTo(1));
        }

        Guid _correlationId;
        Mock<ISagaPolicy<SimpleSaga, InitiateSimpleSaga>> _sagaPolicy;
        Mock<SagaQueryConsumeContext<SimpleSaga, InitiateSimpleSaga>> _sagaQueryConsumeContext;
        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _sagaConsumeContext;
        Mock<IMongoDbSagaConsumeContextFactory> _sagaConsumeContextFactory;

        [OneTimeSetUp]
        public async Task GivenAMongoDbSagaRepository_WhenSendingQuery()
        {
            _correlationId = Guid.NewGuid();
            var saga = new SimpleSaga {CorrelationId = _correlationId};

            await SagaRepository.InsertSaga(saga);

            _sagaQueryConsumeContext = new Mock<SagaQueryConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _sagaQueryConsumeContext.Setup(x => x.Query.FilterExpression).Returns(x => x.CorrelationId == _correlationId);
            _sagaPolicy = new Mock<ISagaPolicy<SimpleSaga, InitiateSimpleSaga>>();
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();

            _sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _sagaConsumeContext.Setup(x => x.CorrelationId).Returns(_correlationId);

            _sagaConsumeContextFactory = new Mock<IMongoDbSagaConsumeContextFactory>();
            _sagaConsumeContextFactory.Setup(
                m =>
                    m.Create(It.IsAny<IMongoCollection<SimpleSaga>>(), _sagaQueryConsumeContext.Object,
                        It.Is<SimpleSaga>(x => x.CorrelationId == _correlationId), true)).Returns(_sagaConsumeContext.Object);

            var repository = new MongoDbSagaRepository<SimpleSaga>(SagaRepository.Instance, _sagaConsumeContextFactory.Object);

            await repository.SendQuery(_sagaQueryConsumeContext.Object, _sagaQueryConsumeContext.Object.Query, _sagaPolicy.Object, _nextPipe.Object);
        }

        [OneTimeTearDown]
        public Task Kill()
        {
            return SagaRepository.DeleteSaga(_correlationId);
        }
    }
}
