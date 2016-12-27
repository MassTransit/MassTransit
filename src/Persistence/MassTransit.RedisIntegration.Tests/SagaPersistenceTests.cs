// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RedisIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RedisInside;
    using Saga;
    using ServiceStack.Redis;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga : InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message).ConfigureAwait(false);

            var found = _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldBeTrue();

            var nextMessage = new CompleteSimpleSaga {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage).ConfigureAwait(false);

            found = await _sagaRepository.Value.ShouldContainSaga(sagaId, x => x.Completed, TestTimeout).ConfigureAwait(false);
            found.ShouldBeTrue();
            var retrieveRepository = _sagaRepository.Value as IRetrieveSagaFromRepository<SimpleSaga>;
            var retrieved = retrieveRepository.GetSaga(sagaId);
            retrieved.ShouldNotBeNull();
            retrieved.Completed.ShouldBeTrue();
        }

        [Test]
        public void An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            InputQueueSendEndpoint.Send(message);

            var found = _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldBeTrue();
        }

        [OneTimeTearDown]
        public void TearDownRedis() => _redis.Dispose();

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;
        readonly Redis _redis;

        public LocatingAnExistingSaga()
        {
            _redis = new Redis();
            var clientManager = new BasicRedisClientManager(_redis.Endpoint.ToString());
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => new RedisSagaRepository<SimpleSaga>(clientManager));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}