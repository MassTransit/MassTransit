namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Linq;
    using NUnit.Framework;


    [TestFixture]
    public class An_address :
        AzureServiceBusTestFixture
    {
        [Test]
        public void Should_get_the_queue_address()
        {
            var address = Host.Topology.GetDestinationAddress("input_queue");

            Assert.That(address, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public void Should_get_the_bus_address()
        {
            var queueName = Bus.Address.AbsolutePath.Split('/').Last();

            var address = Host.Topology.GetDestinationAddress(queueName, x => x.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle);

            Assert.That(address, Is.EqualTo(Bus.Address));
        }
    }
}
