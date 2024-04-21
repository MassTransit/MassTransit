namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Initiating_or_orchestrating_a_consumer_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initiate()
        {
            var sagaId = NewId.NextGuid();

            var message = new EventMessage(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var saga = _sagaHarness.Sagas.Contains(sagaId);
            await Assert.MultipleAsync(async () =>
            {
                Assert.That(saga, Is.Not.Null);

                Assert.That(await _sagaHarness.Consumed.Any<EventMessage>());
            });
        }

        [Test]
        public async Task Should_orchestrate()
        {
            var sagaId = NewId.NextGuid();

            var message = new CreateMessage(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var saga = _sagaHarness.Sagas.Contains(sagaId);
            await Assert.MultipleAsync(async () =>
            {
                Assert.That(saga, Is.Not.Null);

                Assert.That(await _sagaHarness.Consumed.Any<CreateMessage>());
            });

            await InputQueueSendEndpoint.Send(new EventMessage(sagaId));

            Assert.That(await _sagaHarness.Consumed.Any<EventMessage>());
        }

        public Initiating_or_orchestrating_a_consumer_saga()
        {
            _sagaHarness = InMemoryTestHarness.Saga<ConsumerSaga>();
        }

        readonly SagaTestHarness<ConsumerSaga> _sagaHarness;


        public class ConsumerSaga :
            ISaga,
            InitiatedBy<CreateMessage>,
            InitiatedByOrOrchestrates<EventMessage>
        {
            public ConsumerSaga(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public async Task Consume(ConsumeContext<CreateMessage> context)
            {
            }

            public async Task Consume(ConsumeContext<EventMessage> context)
            {
            }

            public Guid CorrelationId { get; set; }
        }


        public class EventMessage :
            CorrelatedBy<Guid>
        {
            public EventMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; private set; }
        }


        public class CreateMessage :
            CorrelatedBy<Guid>
        {
            public CreateMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; private set; }
        }
    }
}
