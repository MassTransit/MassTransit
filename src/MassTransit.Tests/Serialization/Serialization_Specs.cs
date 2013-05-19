using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MassTransit.Tests.Serialization
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;
    using MassTransit.Serialization;

    [TestFixture]
    public class When_publishing_a_message_without_specifying_a_serializer
        : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            ServiceBusFactory.ConfigureDefaultSettings(c => c.SetEndpointCache(null));

            base.EstablishContext();
        }

        [Test]
        public void The_default_serializer_should_be_used()
        {
            var future = new FutureMessage<IConsumeContext<TestMessage>>();

            LocalBus.SubscribeContextHandler<TestMessage>(future.Set);

            LocalBus.Publish(new TestMessage());

            future.IsAvailable(5.Seconds()).ShouldBeTrue("Message not recieved");

            Assert.AreEqual("application/vnd.masstransit+xml", future.Message.ContentType);
        }
    }

    [TestFixture]
    public class When_publishing_a_message_after_specifying_default_serializer
        : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            ServiceBusFactory.ConfigureDefaultSettings(c => c.SetEndpointCache(null));

            base.EstablishContext();
        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.UseJsonSerializer();
        }

        [Test]
        public void The_specified_serializer_should_be_used()
        {
            var future = new FutureMessage<IConsumeContext<TestMessage>>();

            LocalBus.SubscribeContextHandler<TestMessage>(future.Set);

            LocalBus.Publish(new TestMessage());

            future.IsAvailable(5.Seconds()).ShouldBeTrue("Message not recieved");

            Assert.AreEqual("application/vnd.masstransit+json", future.Message.ContentType);
        }
    }

    [TestFixture]
    public class When_publishing_a_message_implicitly_to_an_endpoint_with_a_configured_serializer
        : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            ServiceBusFactory.ConfigureDefaultSettings(c => c.SetEndpointCache(null));
            base.EstablishContext();
        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.ConfigureEndpoint("loopback://localhost/mt_client",
                                           c => c.UseSerializer<BinaryMessageSerializer>());
        }

        [Test]
        public void The_specified_serializer_should_be_used()
        {
            var future = new FutureMessage<IConsumeContext<TestMessage>>();

            LocalBus.SubscribeContextHandler<TestMessage>(future.Set);

            LocalBus.Publish(new TestMessage());

            future.IsAvailable(5.Seconds()).ShouldBeTrue("Message not recieved");

            Assert.AreEqual("application/vnd.masstransit+binary", future.Message.ContentType);
        }
    }

    [TestFixture]
    public class When_sending_a_message_explicitly_to_an_endpoint_with_a_configured_serializer
        : LoopbackLocalAndRemoteTestFixture
    {
        public When_sending_a_message_explicitly_to_an_endpoint_with_a_configured_serializer()
        {
            ConfigureEndpointFactory(f => f.ConfigureEndpoint("loopback://localhost/mt_server",
                                                              c => c.UseSerializer<BinaryMessageSerializer>()));
        }

        [Test]
        public void The_specified_serializer_should_be_used()
        {
            var future = new FutureMessage<IConsumeContext<TestMessage>>();

            RemoteBus.SubscribeContextHandler<TestMessage>(future.Set);
            LocalBus.ShouldHaveRemoteSubscriptionFor<TestMessage>();

            LocalBus.GetEndpoint(new Uri("loopback://localhost/mt_server"))
                .Send(new TestMessage());

            future.IsAvailable(5.Seconds()).ShouldBeTrue("Message not recieved");

            Assert.AreEqual("application/vnd.masstransit+binary", future.Message.ContentType);
        }
    }

    [TestFixture, Ignore("This feature cannot be tested using the Loopback transport")]
    public class When_receiving_a_message_from_a_different_endpoint
        : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            ServiceBusFactory.ConfigureDefaultSettings(c => c.SetEndpointCache(null));
            base.EstablishContext();
        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);
            configurator.ConfigureEndpoint("loopback://localhost/mt_client", c => c.UseSerializer<JsonMessageSerializer>());
        }

        protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);
            configurator.ConfigureEndpoint("loopback://localhost/mt_server", c => c.UseSerializer<BinaryMessageSerializer>());
        }

        [Test]
        public void The_content_type_header_should_be_used_to_determine_serializer()
        {
            var future = new FutureMessage<IConsumeContext<TestMessage>>();

            RemoteBus.SubscribeContextHandler<TestMessage>(future.Set);
            LocalBus.ShouldHaveRemoteSubscriptionFor<TestMessage>();

            LocalBus.Publish(new TestMessage());

            future.IsAvailable(5.Seconds()).ShouldBeTrue("Message not received");

            Assert.AreEqual("application/vnd.masstransit+binary", future.Message.ContentType);
        }
    }

    [Serializable]
    public class TestMessage
    {
    }
}
