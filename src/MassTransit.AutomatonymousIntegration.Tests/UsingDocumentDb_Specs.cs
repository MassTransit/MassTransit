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
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using Automatonymous;
    using DocumentDbIntegration.Saga;
    using DocumentDbIntegration.Tests;
    using GreenPipes;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Saga;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Threading.Tasks;
    using TestFramework;
    using Testing;


    [TestFixture]
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
                x.Handle<DbUpdateException>();
                x.Immediate(5);
            });
            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_DocumentDB()
        {
            _databaseName = "shoppingChoreSagas";
            _collectionName = "sagas";
            _documentClient = new DocumentClient(new Uri(EmulatorConstants.EndpointUri), EmulatorConstants.Key);

            _repository = new Lazy<ISagaRepository<ShoppingChore>>(() => new DocumentDbSagaRepository<ShoppingChore>(_documentClient, _databaseName, _collectionName));
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

        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);
            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new SodOff
            {
                CorrelationId = correlationId
            });

            sagaId = await _repository.Value.ShouldNotContainSaga(correlationId, TestTimeout);
            Assert.IsFalse(sagaId.HasValue);
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new GotHitByACar
            {
                CorrelationId = correlationId
            });

            sagaId = await _repository.Value.ShouldContainSaga(x => x.CorrelationId == correlationId
                && x.CurrentState == _machine.Dead.Name, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

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