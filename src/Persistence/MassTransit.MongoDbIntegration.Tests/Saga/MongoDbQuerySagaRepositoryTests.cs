namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MongoDbIntegration.Saga.Context;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class MongoDbQuerySagaRepositoryTests
    {
        [Test]
        public void ThenCorrelationIdsReturned()
        {
            Assert.That(_result.Saga.CorrelationId, Is.EqualTo(_correlationId));
        }

        Guid _correlationId;
        SagaConsumeContext<SimpleSaga, InitiateSimpleSaga> _result;

        [OneTimeSetUp]
        public async Task GivenAMongoDbQuerySagaRepository_WhenFindingSaga()
        {
            _correlationId = Guid.NewGuid();

            await SagaRepository.InsertSaga(new SimpleSaga {CorrelationId = _correlationId});

            var repository = new MongoDbSagaRepositoryContext<SimpleSaga, InitiateSimpleSaga>(SagaRepository.Instance.GetCollection<SimpleSaga>("sagas"),
                Mock.Of<ConsumeContext<InitiateSimpleSaga>>(), new MongoDbSagaConsumeContextFactory<SimpleSaga>());

            _result = await repository.Load(_correlationId);
        }

        [OneTimeTearDown]
        public Task Kill()
        {
            return SagaRepository.DeleteSaga(_correlationId);
        }
    }
}
