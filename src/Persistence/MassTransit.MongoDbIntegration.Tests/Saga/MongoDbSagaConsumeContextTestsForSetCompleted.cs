namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Saga;
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
        SagaConsumeContext<SimpleSaga, InitiateSimpleSaga> _mongoDbSagaConsumeContext;

        [OneTimeSetUp]
        public async Task GivenAMongoDbSagaConsumeContext_WhenSettingComplete()
        {
            _saga = new SimpleSaga {CorrelationId = Guid.NewGuid()};

            await SagaRepository.InsertSaga(_saga);

            _mongoDbSagaConsumeContext =
                new DefaultSagaConsumeContext<SimpleSaga, InitiateSimpleSaga>(Mock.Of<ConsumeContext<InitiateSimpleSaga>>(), _saga,
                    SagaConsumeContextMode.Insert);

            await _mongoDbSagaConsumeContext.SetCompleted();
        }

        [OneTimeTearDown]
        public Task Kill()
        {
            return SagaRepository.DeleteSaga(_saga.CorrelationId);
        }
    }
}
