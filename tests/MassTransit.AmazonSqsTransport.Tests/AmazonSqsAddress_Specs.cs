namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Given_a_valid_host_address
    {
        [Test]
        public void Should_return_a_valid_address_for_a_full_address()
        {
            var host = new Uri("amazonsqs://remote-host");
            var address = new AmazonSqsHostAddress(host);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scope, Is.EqualTo("/"));
                Assert.That(address.Host, Is.EqualTo("remote-host"));
            });

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(host));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_scope()
        {
            var host = new Uri("amazonsqs://remote-host/production");
            var address = new AmazonSqsHostAddress(host);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scope, Is.EqualTo("production"));
                Assert.That(address.Host, Is.EqualTo("remote-host"));
            });

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(host));
        }
    }


    [TestFixture]
    public class Given_a_valid_endpoint_address
    {
        [Test]
        public void Should_return_a_valid_address_for_a_full_address()
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("amazonsqs://remote-host/input-queue"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scope, Is.EqualTo("/"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));
            });

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://remote-host/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_scope()
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("amazonsqs://remote-host/production/input-queue"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scope, Is.EqualTo("production"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));
            });

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://remote-host/production/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_queue()
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("queue:input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://localhost/test/input-queue")));
        }
    }


    [TestFixture]
    public class Given_a_valid_topic_address
    {
        [Test]
        public void Should_return_a_valid_address_for_a_topic()
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("topic:input"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://localhost/test_input?type=topic")));
        }

        [Theory]
        [TestCase(true)]
        [TestCase(false)]
        public void Should_return_a_valid_address_for_a_temporary_topic(bool isTemporary)
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");
            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri($"topic:input?temporary={isTemporary}"));

            Assert.Multiple(() =>
            {
                Assert.That(address.AutoDelete, Is.EqualTo(isTemporary));
                Assert.That(address.Durable, Is.Not.EqualTo(isTemporary));
            });
        }

        [Theory]
        [TestCase(true)]
        [TestCase(false)]
        public void Should_return_a_valid_address_for_a_durable_topic(bool isDurable)
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");
            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri($"topic:input?durable={isDurable}"));

            Assert.That(address.Durable, Is.EqualTo(isDurable));
        }

        [Theory]
        [TestCase(true)]
        [TestCase(false)]
        public void Should_return_a_valid_address_for_a_auto_delete_topic(bool autoDelete)
        {
            var hostAddress = new Uri("amazonsqs://localhost/test");
            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri($"topic:input?autodelete={autoDelete}"));

            Assert.That(address.AutoDelete, Is.EqualTo(autoDelete));
        }
    }
}
