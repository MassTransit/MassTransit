namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using GreenPipes;
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using Messages;
    using Moq;
    using NUnit.Framework;
    using Newtonsoft.Json;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendQuery
    {
        [Test]
        public async Task ThenVersionIncremented()
        {
            var sagaDocument = await SagaRepository.Instance.GetSagaDocument(_correlationId);

            var etagGuid = JsonConvert.DeserializeObject<Guid>(sagaDocument.ETag);
            Assert.That(etagGuid != Guid.Empty, Is.True);
        }

        Guid _correlationId;
        Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>> _sagaPolicy;
        Mock<SagaQueryConsumeContext<SimpleSagaResource, InitiateSimpleSaga>> _sagaQueryConsumeContext;
        Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>> _nextPipe;
        Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>> _sagaConsumeContext;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingQuery()
        {
            _correlationId = Guid.NewGuid();
            var saga = new SimpleSagaResource {CorrelationId = _correlationId};

            await SagaRepository.Instance.InsertSaga(saga, true);

            _sagaQueryConsumeContext = new Mock<SagaQueryConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>();
            _sagaQueryConsumeContext.Setup(x => x.Query.FilterExpression).Returns(x => x.CorrelationId == _correlationId);
            _sagaPolicy = new Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>>();
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>();

            _sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>();
            _sagaConsumeContext.Setup(x => x.CorrelationId).Returns(_correlationId);

            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName,
                SagaRepository.CollectionName);

            await repository.SendQuery(_sagaQueryConsumeContext.Object, _sagaQueryConsumeContext.Object.Query, _sagaPolicy.Object, _nextPipe.Object);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_correlationId);
        }
    }
}
