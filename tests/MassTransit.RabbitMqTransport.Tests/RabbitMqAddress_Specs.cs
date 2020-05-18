// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Topology;


    [TestFixture]
    public class GivenAVHostAddress
    {
        [Test]
        public void ShouldNotHaveATtl()
        {
            _hostSettings.Host.ShouldBe("some_server");
        }

        [Test]
        public void Should_have_no_password()
        {
            _hostSettings.Password.ShouldBe("");
        }

        [Test]
        public void Should_have_no_username()
        {
            _hostSettings.Username.ShouldBe("");
        }

        [Test]
        public void Should_have_the_queue_name()
        {
            _receiveSettings.QueueName.ShouldBe("queue");
        }

        [Test]
        public void TheHost()
        {
            _hostSettings.VirtualHost.ShouldBe("thehost");
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server/thehost/queue");
        RabbitMqHostSettings _hostSettings;
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _hostSettings = _uri.GetHostSettings();
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class GivenAnAddressWithUnderscore
    {
        [Test]
        public void Should_have_the_queue_name()
        {
            _receiveSettings.QueueName.ShouldBe("the_queue");
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server/thehost/the_queue");
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class GivenAnAddressWithPeriod
    {
        [Test]
        public void Should_have_the_queue_name()
        {
            _receiveSettings.QueueName.ShouldBe("the.queue");
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server/thehost/the.queue");
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class GivenAnAddressWithColon
    {
        [Test]
        public void Should_have_the_queue_name()
        {
            _receiveSettings.QueueName.ShouldBe("the:queue");
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server/thehost/the:queue");
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class Given_a_valid_endpoint_address
    {
        [Test]
        public void Should_return_a_valid_address_for_a_queue()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("queue:input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://localhost/test/input-queue?bind=true")));
        }

        [Test]
        public void Should_return_a_valid_address_for_an_exchange()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("exchange:input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://localhost/test/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("rabbitmq://remote-host/production/client/input-queue"));

            Assert.That(address.VirtualHost, Is.EqualTo("production/client"));
            Assert.That(address.Name, Is.EqualTo("input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://remote-host/production%2Fclient/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_encoded_slash()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("rabbitmq://remote-host/production%2Fclient/input-queue"));

            Assert.That(address.VirtualHost, Is.EqualTo("production/client"));
            Assert.That(address.Name, Is.EqualTo("input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://remote-host/production%2Fclient/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_using_default_virtual_host()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("rabbitmq://remote-host/input-queue"));

            Assert.That(address.VirtualHost, Is.EqualTo("/"));
            Assert.That(address.Name, Is.EqualTo("input-queue"));
            Assert.That(address.Port, Is.EqualTo(5672));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://remote-host/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_with_a_default_port()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("rabbitmq://remote-host:5672/input-queue"));

            Assert.That(address.Port, Is.EqualTo(5672));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://remote-host/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_with_a_custom_port()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("rabbitmq://remote-host:25672/input-queue"));

            Assert.That(address.Port, Is.EqualTo(25672));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmq://remote-host:25672/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_with_a_secure_port()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");

            var address = new RabbitMqEndpointAddress(hostAddress, new Uri("rabbitmqs://remote-host/input-queue"));

            Assert.That(address.Port, Is.EqualTo(5671));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("rabbitmqs://remote-host/input-queue")));
        }
    }



    [TestFixture]
    public class GivenAnAddressWithSlash
    {
        [Test]
        public void Should_have_the_queue_name()
        {
            _receiveSettings = _uri.GetReceiveSettings();

            _receiveSettings.QueueName.ShouldBe("queue");
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server/thehost/the/queue");
        ReceiveSettings _receiveSettings;
    }


    [TestFixture]
    public class GivenANonVHostAddress
    {
        [Test]
        public void TheHost()
        {
            _hostSettings.VirtualHost.ShouldBe("/");
        }

        [Test]
        public void TheQueue()
        {
            _receiveSettings.QueueName.ShouldBe("the_queue");
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server/the_queue");
        readonly Uri _hostUri = new Uri("rabbitmq://some_server/");
        RabbitMqHostSettings _hostSettings;
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _hostSettings = _hostUri.GetHostSettings();
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class GivenAPortedAddress
    {
        [Test]
        public void TheHost()
        {
            _hostSettings.VirtualHost.ShouldBe("/");
        }

        [Test]
        public void ThePort()
        {
            _hostSettings.Port.ShouldBe(12);
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server:12/");
        RabbitMqHostSettings _hostSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _hostSettings = _uri.GetHostSettings();
        }
    }


    [TestFixture]
    public class GivenANonPortedAddress
    {
        [Test]
        public void TheHost()
        {
            _hostSettings.VirtualHost.ShouldBe("/");
        }

        [Test]
        public void ThePort()
        {
            _hostSettings.Port.ShouldBe(5672);
        }

        readonly Uri _uri = new Uri("rabbitmq://some_server");
        RabbitMqHostSettings _hostSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _hostSettings = _uri.GetHostSettings();
        }
    }


    [TestFixture]
    public class GivenATimeToLive
    {
        [Test]
        public void HighAvailabilityQueue()
        {
            _receiveSettings.QueueArguments["x-message-ttl"].ShouldBe("30000");
        }

        [Test]
        public void ShouldHaveATtl()
        {
            _receiveSettings.QueueArguments.ContainsKey("x-message-ttl").ShouldBe(true);
        }

        [Test]
        public void TheQueueArguments()
        {
            _receiveSettings.QueueArguments.ShouldNotBe(null);
        }

        [Test]
        public void TheQueueName()
        {
            _receiveSettings.QueueName.ShouldBe("somequeue");
        }

        readonly Uri _uri = new Uri("rabbitmq://localhost/mttest/somequeue?ttl=30000");
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class Given_a_prefetch_count
    {
        [Test]
        public void Should_have_the_prefetch_count_on_the_address()
        {
            _receiveSettings.PrefetchCount.ShouldBe((ushort)32);
        }

        [Test]
        public void TheQueueArguments()
        {
            _receiveSettings.QueueArguments.ShouldBeEmpty();
        }

        [Test]
        public void TheQueueName()
        {
            _receiveSettings.QueueName.ShouldBe("somequeue");
        }

        readonly Uri _uri = new Uri("rabbitmq://localhost/mttest/somequeue?prefetch=32");
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }


    [TestFixture]
    public class Given_a_temporary_queue_was_requested
    {
        [Test]
        public void Should_be_auto_delete()
        {
            _receiveSettings.AutoDelete.ShouldBe(true);
        }

        [Test]
        public void Should_be_exclusive_to_the_consumer()
        {
            _receiveSettings.Exclusive.ShouldBe(true);
        }

        [Test]
        public void Should_not_be_durable()
        {
            _receiveSettings.Durable.ShouldBe(false);
        }

        [Test]
        public void TheQueueArguments()
        {
            _receiveSettings.QueueArguments.ShouldBeEmpty();
        }

        [Test]
        public void TheQueueName()
        {
            var guid = new Guid(_receiveSettings.QueueName);
            Assert.AreNotEqual(Guid.Empty, guid);
        }

        readonly Uri _uri = new Uri("rabbitmq://localhost/mttest/*?temporary=true");
        ReceiveSettings _receiveSettings;

        [OneTimeSetUp]
        public void WhenParsed()
        {
            _receiveSettings = _uri.GetReceiveSettings();
        }
    }
}
