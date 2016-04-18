namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Threading;
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

        [TestFixtureSetUp]
        public void GivenAMongoDbSagaRespository_WhenSendingCompletedInstance()
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
            TaskUtil.Await(() => _simpleSaga.Consume(It.IsAny<ConsumeContext<CompleteSimpleSaga>>()));
            TaskUtil.Await(() => SagaRepository.InsertSaga(_simpleSaga));

            var sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSaga, CompleteSimpleSaga>>();
            sagaConsumeContext.SetupGet(x => x.IsCompleted).Returns(true);
            var mongoDbSagaConsumeContextFactory = new Mock<IMongoDbSagaConsumeContextFactory>();
            mongoDbSagaConsumeContextFactory.Setup(x => x.Create(It.IsAny<IMongoCollection<SimpleSaga>>(), context.Object, It.IsAny<SimpleSaga>(), true)).Returns(sagaConsumeContext.Object);
            var repository = new MongoDbSagaRepository<SimpleSaga>(SagaRepository.Instance, mongoDbSagaConsumeContextFactory.Object);

            TaskUtil.Await(() => repository.Send(context.Object, Mock.Of<ISagaPolicy<SimpleSaga, CompleteSimpleSaga>>(), null));
        }

        [TestFixtureTearDown]
        public void Kill()
        {
            TaskUtil.Await(() => SagaRepository.DeleteSaga(_correlationId));
        }
    }
}