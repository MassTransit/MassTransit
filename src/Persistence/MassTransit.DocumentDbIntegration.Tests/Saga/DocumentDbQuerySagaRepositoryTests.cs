namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using DocumentDbIntegration.Saga;
    using MassTransit.Saga;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    [TestFixture]
    public class DocumentDbQuerySagaRepositoryTests
    {
        [Test]
        public void ThenCorrelationIdsReturned()
        {
            Assert.That(_result.Single(), Is.EqualTo(_correlationId));
        }

        Guid _correlationId;
        IEnumerable<Guid> _result;

        [OneTimeSetUp]
        public void GivenADocumentDbQuerySagaRepository_WhenFindingSaga()
        {
            _correlationId = Guid.NewGuid();

            SagaRepository.Instance.InsertSaga(new SimpleSaga { CorrelationId = _correlationId }).GetAwaiter().GetResult();

            var repository = new DocumentDbSagaRepository<SimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName);

            ISagaQuery<SimpleSaga> query = new SagaQuery<SimpleSaga>(x => x.CorrelationId == _correlationId);

            _result = repository.Find(query).GetAwaiter().GetResult();
        }

        [OneTimeTearDown]
        public void Kill()
        {
            SagaRepository.Instance.DeleteSaga(_correlationId).GetAwaiter().GetResult();
        }
    }
}
