namespace MassTransit.Tests.StateMachine
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class OrderSaga_Specs
    {
        [Test]
        public void The_saga_should_have_the_proper_initial_state()
        {
            OrderSaga saga = new OrderSaga();

            Assert.AreEqual(OrderSaga.Initial, saga.Current);
        }

        [Test]
        public void The_reception_of_the_initial_message_should_set_the_state()
        {
            OrderSaga saga = new OrderSaga();

            CreateOrder message = new CreateOrder(Guid.NewGuid());

            saga.Consume(message);

            Assert.AreEqual(OrderSaga.Active, saga.Current);
        }
    }

    [Serializable]
    public class CreateOrder :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public CreateOrder(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}