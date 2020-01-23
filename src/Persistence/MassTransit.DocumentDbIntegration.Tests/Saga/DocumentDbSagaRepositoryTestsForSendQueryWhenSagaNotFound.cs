namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using GreenPipes;
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using DocumentDbIntegration.Saga.Context;
    using Messages;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendQueryWhenSagaNotFound
    {
        [Test]
        public void ThenSagaNotSentToInstance()
        {
            _sagaPolicy.Verify(x => x.Existing(It.IsAny<DocumentDbSagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>(), _nextPipe.Object),
                Times.Never);
        }

        Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>> _sagaPolicy;
        Mock<SagaQueryConsumeContext<SimpleSagaResource, InitiateSimpleSaga>> _sagaQueryConsumeContext;
        Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>> _nextPipe;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingQueryAndSagaNotFound()
        {
            _sagaQueryConsumeContext = new Mock<SagaQueryConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>();
            _sagaQueryConsumeContext.Setup(x => x.Query.FilterExpression).Returns(x => x.CorrelationId == Guid.NewGuid());
            _sagaPolicy = new Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>>();
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>();

            var repository =
                new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName);

            await repository.SendQuery(_sagaQueryConsumeContext.Object, _sagaQueryConsumeContext.Object.Query, _sagaPolicy.Object, _nextPipe.Object);
        }
    }
}
