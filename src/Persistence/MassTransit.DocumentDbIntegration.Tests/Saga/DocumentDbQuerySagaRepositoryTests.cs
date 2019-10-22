namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using DocumentDbIntegration.Saga;
    using MassTransit.Saga;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;


    [TestFixture]
    public class DocumentDbQuerySagaRepositoryTests
    {
        private List<Guid> _ids = new List<Guid>();

        [Test]
        public async Task Query_Fails_With_Json_Resolver_Rename()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);
            SagaRepository.Instance.InsertSaga(new SimpleSaga { CorrelationId = correlationId }, true).GetAwaiter().GetResult();
            ISagaQuery<SimpleSaga> query = new SagaQuery<SimpleSaga>(x => x.CorrelationId == correlationId);

            // Act
            var repository = new DocumentDbSagaRepository<SimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, JsonSerializerSettingsExtensions.GetSagaRenameSettings<SimpleSaga>());
            var result = repository.Find(query).GetAwaiter().GetResult();

            // Assert
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public async Task Query_Other_Property_Passes_With_Json_Resolver_Rename()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);
            var username = Guid.NewGuid().ToString(); // wouldn't actually be a guid, but used this for uniqueness
            SagaRepository.Instance.InsertSaga(new SimpleSaga { CorrelationId = correlationId, Username = username }, true).GetAwaiter().GetResult();
            ISagaQuery<SimpleSaga> query = new SagaQuery<SimpleSaga>(x => x.Username == username);

            // Act
            var repository = new DocumentDbSagaRepository<SimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, JsonSerializerSettingsExtensions.GetSagaRenameSettings<SimpleSaga>());
            var result = repository.Find(query).GetAwaiter().GetResult();

            // Assert
            Assert.That(result.Single(), Is.EqualTo(correlationId)); // So it does find it
        }

        [Test]
        public async Task Query_Passes_With_Json_Property_Attribute()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);
            SagaRepository.Instance.InsertSaga(new SimpleSagaResource { CorrelationId = correlationId }, false).GetAwaiter().GetResult();
            ISagaQuery<SimpleSagaResource> query = new SagaQuery<SimpleSagaResource>(x => x.CorrelationId == correlationId);

            // Act
            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName);
            var result = repository.Find(query).GetAwaiter().GetResult();

            // Assert
            Assert.That(result.Single(), Is.EqualTo(correlationId));
        }

        [OneTimeTearDown]
        public void Kill()
        {
            foreach (var id in _ids)
            {
                SagaRepository.Instance.DeleteSaga(id).GetAwaiter().GetResult();
            }
        }
    }
}
