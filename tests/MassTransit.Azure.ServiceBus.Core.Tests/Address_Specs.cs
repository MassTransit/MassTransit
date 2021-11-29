namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Linq;
    using AzureServiceBusTransport;
    using NUnit.Framework;


    [TestFixture]
    public class An_address :
        AzureServiceBusTestFixture
    {
        [Test]
        public void Should_get_the_bus_address()
        {
            var queueName = Bus.Address.AbsolutePath.Split('/').Last();

            var address = Bus.GetServiceBusBusTopology().GetDestinationAddress(queueName, x => x.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle);

            Assert.That(address, Is.EqualTo(Bus.Address));
        }

        [Test]
        public void Should_get_the_queue_address()
        {
            var address = Bus.GetServiceBusBusTopology().GetDestinationAddress("input_queue");

            Assert.That(address, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public void Should_have_the_full_path()
        {
            var address = new ServiceBusEndpointAddress(HostAddress, "input_queue");

            Assert.That((Uri)address, Is.EqualTo(InputQueueAddress));

            Assert.That(address.Name, Is.EqualTo("input_queue"));
            Assert.That(address.Scope, Is.EqualTo(typeof(An_address).Namespace));

            Assert.That(address.Path, Is.EqualTo(InputQueueAddress.AbsolutePath.Substring(1)));
        }
    }
}
