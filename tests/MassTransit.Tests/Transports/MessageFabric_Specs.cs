namespace MassTransit.Tests.Transports
{
    using System;
    using System.Threading.Tasks;
    using InMemoryTransport;
    using MassTransit.Transports.Fabric;
    using NUnit.Framework;


    [TestFixture]
    public class MessageFabric_Specs
    {
        [Test]
        public async Task Should_allow_a_legitimate_binding()
        {
            var fabric = new MessageFabric<InMemoryTransportContext, InMemoryTransportMessage>();

            fabric.ExchangeBind(null, "Namespace.A", "input-exchange", default);
            fabric.ExchangeBind(null, "Namespace.B", "input-exchange", default);
            fabric.QueueBind(null, "input-exchange", "input-queue");
        }

        [Test]
        public async Task Should_not_allow_a_cyclic_binding()
        {
            var fabric = new MessageFabric<InMemoryTransportContext, InMemoryTransportMessage>();

            fabric.ExchangeBind(null, "Namespace.A", "input-exchange", default);
            fabric.ExchangeBind(null, "Namespace.B", "input-exchange", default);

            fabric.ExchangeBind(null, "input-exchange", "output-exchange", default);

            Assert.That(() => fabric.ExchangeBind(null, "output-exchange", "Namespace.A", default), Throws.TypeOf<InvalidOperationException>());
        }
    }
}
