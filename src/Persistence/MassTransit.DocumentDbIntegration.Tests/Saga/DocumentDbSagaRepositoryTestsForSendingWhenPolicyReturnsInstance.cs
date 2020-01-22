namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using GreenPipes;
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using MassTransit.Saga.Factories;
    using MassTransit.Saga.Policies;
    using Messages;
    using Moq;
    using NUnit.Framework;
    using Newtonsoft.Json;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendingWhenPolicyReturnsInstance
    {
        [Test]
        public async Task ThenSagaInstanceStored()
        {
            Assert.That(await SagaRepository.Instance.GetSaga<SimpleSagaResource>(_correlationId, true), Is.Not.Null);
        }

        [Test]
        public async Task ThenVersionIncremented()
        {
            var sagaDocument = await SagaRepository.Instance.GetSagaDocument(_correlationId);

            var etagGuid = JsonConvert.DeserializeObject<Guid>(sagaDocument.ETag);
            Assert.That(etagGuid != Guid.Empty, Is.True);
        }

        Mock<ConsumeContext<InitiateSimpleSaga>> _context;
        SimpleSagaResource _simpleSaga;
        Guid _correlationId;
        CancellationToken _cancellationToken;
        Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>> _nextPipe;
        NewOrExistingSagaPolicy<SimpleSagaResource, InitiateSimpleSaga> _policy;
        DocumentDbSagaRepository<SimpleSagaResource> _repository;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingAndPolicyReturnsInstance()
        {
            _correlationId = Guid.NewGuid();
            _cancellationToken = new CancellationToken();

            _context = new Mock<ConsumeContext<InitiateSimpleSaga>>();
            _context.Setup(x => x.CorrelationId).Returns(_correlationId);
            _context.Setup(m => m.CancellationToken).Returns(_cancellationToken);

            _simpleSaga = new SimpleSagaResource {CorrelationId = _correlationId};

            var sagaFactory = new FactoryMethodSagaFactory<SimpleSagaResource, InitiateSimpleSaga>(x => _simpleSaga);
            _policy = new NewOrExistingSagaPolicy<SimpleSagaResource, InitiateSimpleSaga>(sagaFactory, true);

            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>();

            _repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName,
                SagaRepository.CollectionName);

            await _repository.Send(_context.Object, _policy, _nextPipe.Object);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_correlationId);
        }
    }
}
