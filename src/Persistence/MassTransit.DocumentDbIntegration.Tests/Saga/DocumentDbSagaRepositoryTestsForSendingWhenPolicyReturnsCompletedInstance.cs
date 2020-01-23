namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using Messages;
    using Moq;
    using NUnit.Framework;
    using Microsoft.Azure.Documents;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendingWhenPolicyReturnsCompletedInstance
    {
        [Test, Explicit("Doesn't work without mocking, need to use state machine")]
        public async Task ThenTheCompletedSagaIsNotUpdated()
        {
            var actual = await SagaRepository.Instance.GetSagaDocument(_correlationId);

            Assert.That(actual.ETag, Is.EqualTo(_simpleSagaDocument.ETag));
        }

        SimpleSagaResource _simpleSaga;
        Document _simpleSagaDocument;
        CancellationToken _cancellationToken;
        Guid _correlationId;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingCompletedInstance()
        {
            _correlationId = Guid.NewGuid();
            _cancellationToken = new CancellationToken();

            var context = new Mock<ConsumeContext<CompleteSimpleSaga>>();
            context.Setup(x => x.CorrelationId).Returns(_correlationId);
            context.Setup(m => m.CancellationToken).Returns(_cancellationToken);

            _simpleSaga = new SimpleSagaResource
            {
                CorrelationId = _correlationId
            };
            await _simpleSaga.Consume(It.IsAny<ConsumeContext<CompleteSimpleSaga>>());
            await SagaRepository.Instance.InsertSaga(_simpleSaga, true);
            _simpleSagaDocument = await SagaRepository.Instance.GetSagaDocument(_simpleSaga.CorrelationId);

            var sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSagaResource, CompleteSimpleSaga>>();
            sagaConsumeContext.SetupGet(x => x.IsCompleted).Returns(true);
            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName);

            await repository.Send(context.Object, Mock.Of<ISagaPolicy<SimpleSagaResource, CompleteSimpleSaga>>(), null);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_correlationId);
        }
    }
}
