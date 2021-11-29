namespace MassTransit.Tests.Transports
{
    using System;
    using System.Threading.Tasks;
    using InMemoryTransport.Fabric;
    using NUnit.Framework;


    [TestFixture]
    public class MessageFabric_Specs
    {
        [Test]
        public async Task Should_allow_a_legitimate_binding()
        {
            var fabric = new MessageFabric(16);

            fabric.ExchangeBind("Namespace.A", "input-exchange");
            fabric.ExchangeBind("Namespace.B", "input-exchange");
            fabric.QueueBind("input-exchange", "input-queue");
        }

        [Test]
        public async Task Should_not_allow_a_cyclic_binding()
        {
            var fabric = new MessageFabric(16);

            fabric.ExchangeBind("Namespace.A", "input-exchange");
            fabric.ExchangeBind("Namespace.B", "input-exchange");

            fabric.ExchangeBind("input-exchange", "output-exchange");

            Assert.That(() => fabric.ExchangeBind("output-exchange", "Namespace.A"), Throws.TypeOf<InvalidOperationException>());
        }
    }
}
