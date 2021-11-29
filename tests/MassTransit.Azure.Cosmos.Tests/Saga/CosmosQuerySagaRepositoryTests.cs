namespace MassTransit.Azure.Cosmos.Tests.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureCosmos.Saga;
    using Data;
    using MassTransit.Saga;
    using NUnit.Framework;


    [TestFixture]
    public class CosmosQuerySagaRepositoryTests
    {
        [Test]
        public async Task Query_Fails_With_Json_Resolver_Rename()
        {
            var correlationId = Guid.NewGuid();
            _ids.Add(correlationId);

            await Data.SagaRepository<SimpleSaga>.Instance.InsertSaga(new SimpleSaga {CorrelationId = correlationId});

            ISagaRepository<SimpleSaga> repository = CosmosSagaRepository<SimpleSaga>.Create(Data.SagaRepository<SimpleSaga>.Instance.Client,
                Data.SagaRepository<SimpleSaga>.DatabaseName, Data.SagaRepository<SimpleSaga>.CollectionName);

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
            await Data.SagaRepository<SimpleSaga>.Instance.InsertSaga(new SimpleSaga
            {
                CorrelationId = correlationId,
                Username = username
            });

            ISagaRepository<SimpleSaga> repository = CosmosSagaRepository<SimpleSaga>.Create(Data.SagaRepository<SimpleSaga>.Instance.Client,
                Data.SagaRepository<SimpleSaga>.DatabaseName, Data.SagaRepository<SimpleSaga>.CollectionName);

            var querySagaRepository = repository as IQuerySagaRepository<SimpleSaga>;

            IEnumerable<Guid> result = await querySagaRepository.Find(new SagaQuery<SimpleSaga>(x => x.Username == username));

            // Assert
            Assert.That(result.Single(), Is.EqualTo(correlationId)); // So it does find it
        }

        readonly List<Guid> _ids = new List<Guid>();

        //[Test]
        //public async Task Query_Passes_With_Json_Property_Attribute()
        //{
        //    var correlationId = Guid.NewGuid();
        //    _ids.Add(correlationId);

        //    await Data.SagaRepository<SimpleSaga>.Instance.InsertSaga(new SimpleSaga {CorrelationId = correlationId});

        //    var repository = CosmosSagaRepository<SimpleSagaResource>.Create(Data.SagaRepository<SimpleSaga>.Instance.Client, Data.SagaRepository<SimpleSaga>.DatabaseName);

        //    var querySagaRepository = repository as IQuerySagaRepository<SimpleSagaResource>;

        //    var result = await querySagaRepository.Find(new SagaQuery<SimpleSagaResource>(x => x.CorrelationId == correlationId));

        //    Assert.That(result.Single(), Is.EqualTo(correlationId));
        //}

        [OneTimeTearDown]
        public async Task Kill()
        {
            foreach (var id in _ids)
                await Data.SagaRepository<SimpleSaga>.Instance.DeleteSaga(id);
        }
    }
}
