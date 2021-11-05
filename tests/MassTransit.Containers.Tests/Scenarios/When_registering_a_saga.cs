namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using Shouldly;
    using TestFramework;
    using Testing;


    public abstract class When_registering_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_a_subscription_for_the_first_saga_message()
        {
            var sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task Should_have_a_subscription_for_the_second_saga_message()
        {
            var sagaId = NewId.NextGuid();

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
            var sagaId = NewId.NextGuid();

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
