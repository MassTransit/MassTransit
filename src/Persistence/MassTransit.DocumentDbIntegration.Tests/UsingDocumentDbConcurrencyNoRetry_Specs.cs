// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.DocumentDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DocumentDbIntegration.Saga;
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("DocumentDb")]
    public class When_using_DocumentDbConcurrencyNoRetry :
        InMemoryTestFixture
    {
        ChoirStateMachine _machine;
        readonly DocumentClient _documentClient;
        readonly string _databaseName;
        readonly string _collectionName;
        readonly Lazy<ISagaRepository<ChoirStateOptimistic>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new ChoirStateMachine();

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_DocumentDbConcurrencyNoRetry()
        {
            _databaseName = "choirSagas";
            _collectionName = "sagas";
            _documentClient = new DocumentClient(new Uri(EmulatorConstants.EndpointUri), EmulatorConstants.Key);

            _repository = new Lazy<ISagaRepository<ChoirStateOptimistic>>(() => new DocumentDbSagaRepository<ChoirStateOptimistic>(_documentClient, _databaseName, JsonSerializerSettingsExtensions.GetSagaRenameSettings<ChoirStateOptimistic>()));
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

        async Task<ChoirStateOptimistic> GetSaga(Guid id)
        {
            try
            {
                var document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
                return JsonConvert.DeserializeObject<ChoirStateOptimistic>(document.Resource.ToString());
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<ChoirStateOptimistic> GetSagaRetry(Guid id, TimeSpan timeout, Func<ChoirStateOptimistic, bool> filterExpression = null)
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    var document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
                    var saga = JsonConvert.DeserializeObject<ChoirStateOptimistic>(document.Resource.ToString());

                    if (filterExpression?.Invoke(saga) == false) continue;
                    return saga;
                }
                catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await Task.Delay(10).ConfigureAwait(false);

                    continue;
                }
            }

            return null;
        }

        [Test]
        public async Task Some_should_not_be_in_final_state_all()
        {
            var tasks = new List<Task>();

            Guid[] sagaIds = new Guid[20];
            for (int i = 0; i < 20; i++)
            {
                Guid correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

                sagaIds[i] = correlationId;
            }

            for (int i = 0; i < 20; i++)
            {
                var saga = await GetSagaRetry(sagaIds[i], TestTimeout);
                Assert.IsNotNull(saga);
            }

            for (int i = 0; i < 20; i++)
            {
                tasks.Add(InputQueueSendEndpoint.Send(new Bass { CorrelationId = sagaIds[i], Name = "John" }));
                tasks.Add(InputQueueSendEndpoint.Send(new Baritone { CorrelationId = sagaIds[i], Name = "Mark" }));
                tasks.Add(InputQueueSendEndpoint.Send(new Tenor { CorrelationId = sagaIds[i], Name = "Anthony" }));
                tasks.Add(InputQueueSendEndpoint.Send(new Countertenor { CorrelationId = sagaIds[i], Name = "Tom" }));
            }

            await Task.WhenAll(tasks);
            await Task.Delay(2000);
            tasks.Clear();

            var someNotInFinalState = false;

            foreach (var sid in sagaIds)
            {
                ChoirStateOptimistic instance = await GetSaga(sid);

                someNotInFinalState = !instance.CurrentState.Equals("Harmony");
            }

            Assert.IsTrue(someNotInFinalState);
        }

        [Test]
        public async Task Should_not_be_in_final_state()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

            var saga = await GetSagaRetry(correlationId, TestTimeout);

            Assert.IsNotNull(saga);

            await Task.WhenAll(
                InputQueueSendEndpoint.Send(new Bass { CorrelationId = correlationId, Name = "John" }),
                InputQueueSendEndpoint.Send(new Baritone { CorrelationId = correlationId, Name = "Mark" }),
                InputQueueSendEndpoint.Send(new Tenor { CorrelationId = correlationId, Name = "Anthony" }),
                InputQueueSendEndpoint.Send(new Countertenor { CorrelationId = correlationId, Name = "Tom" })
                );

            // Because concurrency exception's happened without retry middleware configured, we aren't in our final state/
            ChoirStateOptimistic instance = await GetSaga(correlationId);

            Assert.IsTrue(!instance.CurrentState.Equals("Harmony"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.TransportConcurrencyLimit = 16;
        }
    }
}
