namespace MassTransit.DbTransport.Tests
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Specifying_a_host_address
    {
        [Test]
        public void Should_support_a_virtual_host()
        {
            var uri = new Uri("db://localhost/customer_a");
            var address = new SqlHostAddress(uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope()
        {
            var uri = new Uri("db://localhost/customer_a.billing");
            var address = new SqlHostAddress(uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_option_2()
        {
            var uri = new Uri("db://localhost/customer_a.billing/");
            var address = new SqlHostAddress(uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://localhost/customer_a.billing")));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_with_queue()
        {
            var uri = new Uri("db://localhost/customer_a.billing/input-queue");

            var address = new SqlHostAddress(uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://localhost/customer_a.billing")));
            });
        }

        [Test]
        public void Should_support_the_simplest_use_case()
        {
            var uri = new Uri("db://localhost");
            var address = new SqlHostAddress(uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("/"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_support_the_simplest_use_case_option_2()
        {
            var uri = new Uri("db://localhost/");
            var address = new SqlHostAddress(uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("/"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_throw_on_invalid_scope()
        {
            Assert.That(() => new SqlHostAddress(new Uri("db://localhost/customer.16/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }

        [Test]
        public void Should_throw_on_invalid_virtual_host()
        {
            Assert.That(() => new SqlHostAddress(new Uri("db://localhost/customer-a.billing/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }
    }


    [TestFixture]
    public class Specifying_an_endpoint_address
    {
        [Test]
        public void Should_parse_the_instance_name_from_url()
        {
            var address = new SqlHostAddress(new Uri("db://localhost/customer_a?instance=instance"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.InstanceName, Is.EqualTo("instance"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://localhost/customer_a?instance=instance")));
            });
        }

        [Test]
        public void Should_support_a_virtual_host()
        {
            var uri = new Uri("db://localhost/customer_a/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a")), uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope()
        {
            var uri = new Uri("db://localhost/customer_a.billing/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing")), uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_option_2()
        {
            var uri = new Uri("db://localhost/customer_a.billing/input-queue");
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing/")), uri);

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_with_queue()
        {
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing/")),
                new Uri("queue:input-queue"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://localhost/customer_a.billing/input-queue")));
            });
        }

        [Test]
        public void Should_support_a_virtual_host_and_scope_with_topic()
        {
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer_a.billing/")),
                new Uri("topic:namespace:type"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.Null);
                Assert.That(address.Name, Is.EqualTo("namespace:type"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://localhost/customer_a/namespace:type?type=topic")));
            });
        }

        [Test]
        public void Should_support_the_backslash_in_sql_host_names()
        {
            var hostAddress = new SqlHostAddress("localhost", "instance", default, "customer_a", "billing");
            var address = new SqlEndpointAddress(hostAddress, new Uri("queue:input-queue"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.InstanceName, Is.EqualTo("instance"));
                Assert.That(address.VirtualHost, Is.EqualTo("customer_a"));
                Assert.That(address.Area, Is.EqualTo("billing"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://localhost/customer_a.billing/input-queue?instance=instance")));
            });
        }

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

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("localhost"));
                Assert.That(address.VirtualHost, Is.EqualTo("/"));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(uri));
            });
        }

        [Test]
        public void Should_throw_on_invalid_scope()
        {
            Assert.That(() => new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer.16")),
                new Uri("db://localhost/customer.16/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }

        [Test]
        public void Should_throw_on_invalid_virtual_host()
        {
            Assert.That(() => new SqlEndpointAddress(new SqlHostAddress(new Uri("db://localhost/customer-a.billing")),
                new Uri("db://localhost/customer-a.billing/input-queue")), Throws.InstanceOf<SqlEndpointAddressException>());
        }

        [Test]
        public void Should_support_ipv6_address()
        {
            var address = new SqlEndpointAddress(new SqlHostAddress(new Uri("db://[::1]:1433/")), new Uri("queue:input-queue"));

            Assert.Multiple(() =>
            {
                Assert.That(address.Scheme, Is.EqualTo("db"));
                Assert.That(address.Host, Is.EqualTo("[::1]"));
                Assert.That(address.Port, Is.EqualTo(1433));
                Assert.That(address.Name, Is.EqualTo("input-queue"));

                Assert.That((Uri)address, Is.EqualTo(new Uri("db://[::1]:1433/input-queue")));
            });
        }
    }
}
