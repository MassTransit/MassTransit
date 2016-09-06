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
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MongoDbIntegration.Saga;
    using MongoDbIntegration.Saga.Context;
    using MongoDB.Driver;
    using Moq;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class MongoDbSagaRepositoryTestsForSendingWhenPolicyReturnsCompletedInstance
    {
        [Test]
        public void ThenTheCompletedSagaIsNotUpdated()
        {
            var actual = TaskUtil.Await(() => SagaRepository.GetSaga(_correlationId));

            Assert.That(actual.Version, Is.EqualTo(_simpleSaga.Version));
        }

        SimpleSaga _simpleSaga;
        CancellationToken _cancellationToken;
        Guid _correlationId;

        [OneTimeSetUp]
        public async Task GivenAMongoDbSagaRespository_WhenSendingCompletedInstance()
        {
            _correlationId = Guid.NewGuid();
            _cancellationToken = new CancellationToken();

            var context = new Mock<ConsumeContext<CompleteSimpleSaga>>();
            context.Setup(x => x.CorrelationId).Returns(_correlationId);
            context.Setup(m => m.CancellationToken).Returns(_cancellationToken);

            _simpleSaga = new SimpleSaga
            {
                CorrelationId = _correlationId,
                Version = 5
            };
            await _simpleSaga.Consume(It.IsAny<ConsumeContext<CompleteSimpleSaga>>());
            await SagaRepository.InsertSaga(_simpleSaga);

            var sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSaga, CompleteSimpleSaga>>();
            sagaConsumeContext.SetupGet(x => x.IsCompleted).Returns(true);
            var mongoDbSagaConsumeContextFactory = new Mock<IMongoDbSagaConsumeContextFactory>();
            mongoDbSagaConsumeContextFactory.Setup(x => x.Create(It.IsAny<IMongoCollection<SimpleSaga>>(), context.Object, It.IsAny<SimpleSaga>(), true))
                .Returns(sagaConsumeContext.Object);
            var repository = new MongoDbSagaRepository<SimpleSaga>(SagaRepository.Instance, mongoDbSagaConsumeContextFactory.Object);

            await repository.Send(context.Object, Mock.Of<ISagaPolicy<SimpleSaga, CompleteSimpleSaga>>(), null);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.DeleteSaga(_correlationId);
        }
    }
}