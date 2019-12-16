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
namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Given_a_valid_host_address
    {
        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_scope_and_virtual_host()
        {
            var host = new Uri("amazonsqs://remote-host/us/production");
            var address = new AmazonSqsHostAddress(host);

            Assert.That(address.Scope, Is.EqualTo("us"));
            Assert.That(address.VirtualHost, Is.EqualTo("production"));
            Assert.That(address.Host, Is.EqualTo("remote-host"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(host));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_scope()
        {
            var host = new Uri("amazonsqs://remote-host/us");
            var address = new AmazonSqsHostAddress(host);

            Assert.That(address.Scope, Is.EqualTo("us"));
            Assert.That(address.VirtualHost, Is.EqualTo("/"));
            Assert.That(address.Host, Is.EqualTo("remote-host"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(host));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address()
        {
            var host = new Uri("amazonsqs://remote-host");
            var address = new AmazonSqsHostAddress(host);

            Assert.That(address.Scope, Is.EqualTo("/"));
            Assert.That(address.VirtualHost, Is.EqualTo("/"));
            Assert.That(address.Host, Is.EqualTo("remote-host"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(host));
        }
    }


    [TestFixture]
    public class Given_a_valid_endpoint_address
    {
        [Test]
        public void Should_return_a_valid_address_for_a_queue()
        {
            var hostAddress = new Uri("amazonsqs://docker.localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("queue:input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://docker.localhost/test/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_scope_and_virtual_host()
        {
            var hostAddress = new Uri("amazonsqs://docker.localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("amazonsqs://remote-host/us/production/input-queue"));

            Assert.That(address.Scope, Is.EqualTo("us"));
            Assert.That(address.VirtualHost, Is.EqualTo("production"));
            Assert.That(address.Name, Is.EqualTo("input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://remote-host/us/production/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address_with_scope()
        {
            var hostAddress = new Uri("amazonsqs://docker.localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("amazonsqs://remote-host/us/input-queue"));

            Assert.That(address.Scope, Is.EqualTo("us"));
            Assert.That(address.VirtualHost, Is.EqualTo("/"));
            Assert.That(address.Name, Is.EqualTo("input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://remote-host/us/input-queue")));
        }

        [Test]
        public void Should_return_a_valid_address_for_a_full_address()
        {
            var hostAddress = new Uri("amazonsqs://docker.localhost/test");

            var address = new AmazonSqsEndpointAddress(hostAddress, new Uri("amazonsqs://remote-host/input-queue"));

            Assert.That(address.Scope, Is.EqualTo("/"));
            Assert.That(address.VirtualHost, Is.EqualTo("/"));
            Assert.That(address.Name, Is.EqualTo("input-queue"));

            Uri uri = address;

            Assert.That(uri, Is.EqualTo(new Uri("amazonsqs://remote-host/input-queue")));
        }
    }
}
