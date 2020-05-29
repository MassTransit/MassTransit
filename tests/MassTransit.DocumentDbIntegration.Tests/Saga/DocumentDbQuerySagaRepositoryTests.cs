namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using DocumentDbIntegration.Saga;
    using MassTransit.Saga;
    using NUnit.Framework;


    [TestFixture]
    public class DocumentDbQuerySagaRepositoryTests
    {
        [Test]
        public async Task Query_Fails_With_Json_Resolver_Rename()
        {
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);

            await SagaRepository.Instance.InsertSaga(new SimpleSaga {CorrelationId = correlationId}, true);

            ISagaRepository<SimpleSaga> repository = DocumentDbSagaRepository<SimpleSaga>.Create(SagaRepository.Instance.Client, SagaRepository.DatabaseName,
                JsonSerializerSettingsExtensions.GetSagaRenameSettings<SimpleSaga>());

            var querySagaRepository = repository as IQuerySagaRepository<SimpleSaga>;

            IEnumerable<Guid> result = await querySagaRepository.Find(new SagaQuery<SimpleSaga>(x => x.CorrelationId == correlationId));

            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public async Task Query_Other_Property_Passes_With_Json_Resolver_Rename()
        {
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);

            var username = Guid.NewGuid().ToString();
            await SagaRepository.Instance.InsertSaga(new SimpleSaga
            {
                CorrelationId = correlationId,
                Username = username
            }, true);

            ISagaRepository<SimpleSaga> repository = DocumentDbSagaRepository<SimpleSaga>.Create(SagaRepository.Instance.Client, SagaRepository.DatabaseName,
                JsonSerializerSettingsExtensions.GetSagaRenameSettings<SimpleSaga>());

            var querySagaRepository = repository as IQuerySagaRepository<SimpleSaga>;

            IEnumerable<Guid> result = await querySagaRepository.Find(new SagaQuery<SimpleSaga>(x => x.Username == username));

            // Assert
            Assert.That(result.Single(), Is.EqualTo(correlationId)); // So it does find it
        }

        [Test]
        public async Task Query_Passes_With_Json_Property_Attribute()
        {
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);

            await SagaRepository.Instance.InsertSaga(new SimpleSagaResource {CorrelationId = correlationId}, false);

            ISagaRepository<SimpleSagaResource> repository =
                DocumentDbSagaRepository<SimpleSagaResource>.Create(SagaRepository.Instance.Client, SagaRepository.DatabaseName);

            var querySagaRepository = repository as IQuerySagaRepository<SimpleSagaResource>;

            IEnumerable<Guid> result = await querySagaRepository.Find(new SagaQuery<SimpleSagaResource>(x => x.CorrelationId == correlationId));

            Assert.That(result.Single(), Is.EqualTo(correlationId));
        }

        readonly List<Guid> _ids = new List<Guid>();

        [OneTimeTearDown]
        public async Task Kill()
        {
            foreach (var id in _ids)
                await SagaRepository.Instance.DeleteSaga(id);
        }
    }
}
