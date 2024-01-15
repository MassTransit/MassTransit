namespace MassTransit.Tests.ContainerTests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


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

            Assert.That(foundId.HasValue, Is.True);
        }

        [Test]
        public async Task Should_have_a_subscription_for_the_second_saga_message()
        {
            var sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);

            var nextMessage = new SecondSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(x => x.CorrelationId == sagaId && x.Second.IsCompleted, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);
        }

        [Test]
        public async Task Should_have_a_subscription_for_the_third_saga_message()
        {
            var sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);

            var nextMessage = new ThirdSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(x => x.CorrelationId == sagaId && x.Third.IsCompleted, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);
        }

        protected abstract ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga;
    }
}
