namespace MassTransit.DocumentDbIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using DocumentDbIntegration.Saga;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("DocumentDb")]
    public class When_using_DocumentDB :
        InMemoryTestFixture
    {
        SuperShopper _machine;
        readonly DocumentClient _documentClient;
        readonly string _databaseName;
        readonly string _collectionName;
        readonly Lazy<ISagaRepository<ShoppingChore>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            configurator.UseRetry(x =>
            {
                x.Immediate(5);
            });
            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_DocumentDB()
        {
            _databaseName = "shoppingChoreSagas";
            _collectionName = "sagas";
            _documentClient = new DocumentClient(EmulatorConstants.EndpointUri, EmulatorConstants.Key);

            _repository = new Lazy<ISagaRepository<ShoppingChore>>(() => new DocumentDbSagaRepository<ShoppingChore>(_documentClient, _databaseName, JsonSerializerSettingsExtensions.GetSagaRenameSettings<ShoppingChore>()));
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await _documentClient.OpenAsync();
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName }).ConfigureAwait(false);
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName), new DocumentCollection { Id = _collectionName }).ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _documentClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName)).ConfigureAwait(false);
            await _documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName)).ConfigureAwait(false);
        }

        async Task<ShoppingChore> GetSaga(Guid id)
        {
            try
            {
                var document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
                return JsonConvert.DeserializeObject<ShoppingChore>(document.Resource.ToString());
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<ShoppingChore> GetNoSagaRetry(Guid id, TimeSpan timeout)
        {
            DateTime giveUpAt = DateTime.Now + timeout;
            ShoppingChore saga = null;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    var document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
                    saga = JsonConvert.DeserializeObject<ShoppingChore>(document.Resource.ToString());

                    await Task.Delay(10).ConfigureAwait(false);
                }
                catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    saga = null;
                    break;
                }
            }

            return saga;
        }

        async Task<ShoppingChore> GetSagaRetry(Guid id, TimeSpan timeout, Func<ShoppingChore, bool> filterExpression = null)
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    var document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
                    var saga = JsonConvert.DeserializeObject<ShoppingChore>(document.Resource.ToString());

                    if (filterExpression?.Invoke(saga) == false) continue;
                    return saga;
                }
                catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await Task.Delay(10).ConfigureAwait(false);
                }
            }

            return null;
        }

        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            var saga = await GetSagaRetry(correlationId, TestTimeout);
            Assert.IsNotNull(saga);

            await InputQueueSendEndpoint.Send(new SodOff
            {
                CorrelationId = correlationId
            });

            saga = await GetNoSagaRetry(correlationId, TestTimeout);
            Assert.IsNull(saga);
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            var saga = await GetSagaRetry(correlationId, TestTimeout);

            Assert.IsNotNull(saga);

            await InputQueueSendEndpoint.Send(new GotHitByACar
            {
                CorrelationId = correlationId
            });

            saga = await GetSagaRetry(correlationId, TestTimeout, x=>x.CurrentState == _machine.Dead.Name);

            Assert.IsNotNull(saga);
            Assert.IsTrue(saga.CurrentState == _machine.Dead.Name);

            ShoppingChore instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.TransportConcurrencyLimit = 16;
        }
    }
}
