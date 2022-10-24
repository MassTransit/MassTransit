namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Linq;
    using AzureServiceBusTransport;
    using Internals;
    using NUnit.Framework;
    using TestFramework.Messages;


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

        [Test]
        public void Should_handle_address_loopback()
        {
            Assert.IsTrue(Bus.Topology.TryGetPublishAddress<PingMessage>(out var address));

            Assert.That(address.TryGetValueFromQueryString("type", out var type), Is.True);
            Assert.That(type, Is.EqualTo("topic"));

            Uri normalizedAddress = new ServiceBusEndpointAddress(AzureServiceBusTestHarness.HostAddress, address);

            Assert.That(normalizedAddress.TryGetValueFromQueryString("type", out type), Is.True);
            Assert.That(type, Is.EqualTo("topic"));

            Assert.That(normalizedAddress.AbsolutePath, Is.EqualTo(address.AbsolutePath));
        }

        [Test]
        public void Should_handle_a_topic_address()
        {
            var address = new ServiceBusEndpointAddress(AzureServiceBusTestHarness.HostAddress, new Uri("topic:private-topic"));

            Assert.That(address.Type, Is.EqualTo(ServiceBusEndpointAddress.AddressType.Topic));

            Uri uri = address;
            Assert.That(uri.TryGetValueFromQueryString("type", out var type), Is.True);
            Assert.That(type, Is.EqualTo("topic"));

            Assert.That(uri.AbsolutePath, Is.EqualTo("/private-topic"));
        }
    }
}
