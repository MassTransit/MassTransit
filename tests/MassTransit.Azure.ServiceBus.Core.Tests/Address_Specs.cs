namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using Internals;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class An_address :
        AzureServiceBusTestFixture
    {
        [Test]
        public void Should_handle_a_topic_address()
        {
            var address = new ServiceBusEndpointAddress(AzureServiceBusTestHarness.HostAddress, new Uri("topic:private-topic"));

            Assert.That(address.Type, Is.EqualTo(ServiceBusEndpointAddress.AddressType.Topic));

            Uri uri = address;
            Assert.Multiple(() =>
            {
                Assert.That(uri.TryGetValueFromQueryString("type", out var type), Is.True);
                Assert.That(type, Is.EqualTo("topic"));

                Assert.That(uri.AbsolutePath, Is.EqualTo("/private-topic"));
            });
        }

        [Test]
        public void Should_handle_address_loopback()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Bus.Topology.TryGetPublishAddress<PingMessage>(out var address), Is.True);

                Assert.That(address.TryGetValueFromQueryString("type", out var type), Is.True);
                Assert.That(type, Is.EqualTo("topic"));

                Uri normalizedAddress = new ServiceBusEndpointAddress(AzureServiceBusTestHarness.HostAddress, address);

                Assert.Multiple(() =>
                {
                    Assert.That(normalizedAddress.TryGetValueFromQueryString("type", out type), Is.True);
                    Assert.That(type, Is.EqualTo("topic"));

                    Assert.That(normalizedAddress.AbsolutePath, Is.EqualTo(address.AbsolutePath));
                });
            });
        }

        [Test]
        public void Should_have_the_full_path()
        {
            var address = new ServiceBusEndpointAddress(HostAddress, "input_queue");

            Assert.Multiple(() =>
            {
                Assert.That((Uri)address, Is.EqualTo(InputQueueAddress));

                Assert.That(address.Name, Is.EqualTo("input_queue"));
                Assert.That(address.Scope, Is.EqualTo(typeof(An_address).Namespace));

                Assert.That(address.Path, Is.EqualTo(InputQueueAddress.AbsolutePath.Substring(1)));
            });
        }
    }
}
