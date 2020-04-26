// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using Shouldly;
    using Testing;


    public abstract class When_registering_a_saga :
        Given_a_service_bus_instance
    {
        [Test]
        public async Task Should_have_a_subscription_for_the_first_saga_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task Should_have_a_subscription_for_the_second_saga_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new SecondSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(x => x.CorrelationId == sagaId && x.Second.IsCompleted, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task Should_have_a_subscription_for_the_third_saga_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new ThirdSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(x => x.CorrelationId == sagaId && x.Third.IsCompleted, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        protected abstract ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga;
    }
}