namespace MassTransit.DbTransport.Tests
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Specifying_a_host_address
    {
        [Test]
        public void Should_support_the_simplest_use_case()
        {
            var uri = new Uri("db://localhost");
            var address = new SqlHostAddress(uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("/", address.VirtualHost);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_the_simplest_use_case_option_2()
        {
            var uri = new Uri("db://localhost/");
            var address = new SqlHostAddress(uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("/", address.VirtualHost);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host()
        {
            var uri = new Uri("db://localhost/customer_a");
            var address = new SqlHostAddress(uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope()
        {
            var uri = new Uri("db://localhost/customer_a.billing");
            var address = new SqlHostAddress(uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("billing", address.Area);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_option_2()
        {
            var uri = new Uri("db://localhost/customer_a.billing/");
            var address = new SqlHostAddress(uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("billing", address.Area);

            Assert.AreEqual(new Uri("db://localhost/customer_a.billing"), (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_with_queue()
        {
            var uri = new Uri("db://localhost/customer_a.billing/input-queue");

            var address = new SqlHostAddress(uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("billing", address.Area);

            Assert.AreEqual(new Uri("db://localhost/customer_a.billing"), (Uri)address);
        }

        [Test]
        public void Should_throw_on_invalid_virtual_host()
        {
            Assert.That(() => new SqlHostAddress(new Uri("db://localhost/customer-a.billing/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }

        [Test]
        public void Should_throw_on_invalid_scope()
        {
            Assert.That(() => new SqlHostAddress(new Uri("db://localhost/customer.16/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }
    }


    [TestFixture]
    public class Specifying_an_endpoint_address
    {
        [Test]
        public void Should_support_the_simplest_use_case()
        {
            Assert.That(() => new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost")),
                new Uri("db://localhost")), Throws.InstanceOf<SqlEndpointAddressException>());
        }

        [Test]
        public void Should_support_the_simplest_use_case_option_2()
        {
            var uri = new Uri("db://localhost/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost")), uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("/", address.VirtualHost);
            Assert.AreEqual("input-queue", address.Name);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host()
        {
            var uri = new Uri("db://localhost/customer_a/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a")), uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("input-queue", address.Name);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope()
        {
            var uri = new Uri("db://localhost/customer_a.billing/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing")), uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("billing", address.Area);
            Assert.AreEqual("input-queue", address.Name);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_option_2()
        {
            var uri = new Uri("db://localhost/customer_a.billing/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing/")), uri);

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("billing", address.Area);
            Assert.AreEqual("input-queue", address.Name);

            Assert.AreEqual(uri, (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_with_queue()
        {
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing/")),
                new Uri("queue:input-queue"));

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.AreEqual("billing", address.Area);
            Assert.AreEqual("input-queue", address.Name);

            Assert.AreEqual(new Uri("db://localhost/customer_a.billing/input-queue"), (Uri)address);
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_with_topic()
        {
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing/")),
                new Uri("topic:namespace:type"));

            Assert.AreEqual("db", address.Scheme);
            Assert.AreEqual("localhost", address.Host);
            Assert.AreEqual("customer_a", address.VirtualHost);
            Assert.IsNull(address.Area);
            Assert.AreEqual("namespace:type", address.Name);

            Assert.AreEqual(new Uri("db://localhost/customer_a/namespace:type?type=topic"), (Uri)address);
        }

        [Test]
        public void Should_throw_on_invalid_virtual_host()
        {
            Assert.That(() => new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer-a.billing")),
                new Uri("db://localhost/customer-a.billing/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }

        [Test]
        public void Should_throw_on_invalid_scope()
        {
            Assert.That(() => new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer.16")),
                new Uri("db://localhost/customer.16/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }
    }
}
