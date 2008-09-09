namespace MassTransit.ServiceBus.Tests.Configuration
{
    using System;
    using MassTransit.ServiceBus.Configuration;
    using MassTransit.ServiceBus.Formatters;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using Transports;

    [TestFixture]
    public class Config_spec :
        Specification
    {
        [Test]
        public void Minimal_Setup()
        {
            Uri listenUri = new Uri("msmq://localhost/bill");

            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn("msmq://localhost/bill")
                .CommunicatesOn<LoopbackEndpoint>() //TODO: what should the default be?
                .Validate();

            Assert.AreEqual(listenUri, busOptions.ListensOn);
            Assert.AreEqual(EndpointResolver.Null.Uri, busOptions.CommandedOn);
            Assert.AreEqual(typeof(BinaryBodyFormatter), busOptions.Serialization.Serializer); //how to handle this?
            Assert.AreEqual(false, busOptions.IsACompetingConsumer);
            Assert.IsNull(busOptions.Subcriptions);
            //transports
        }

        [Test]
        public void As_a_competing_consumer()
        {
            var busOptions = BusBuilder.WithName("funk_bus")
                .AsACompetingConsumer()
                .Validate();

            Assert.AreEqual(true, busOptions.IsACompetingConsumer);
        }

        [Test]
        public void As_a_subscriber()
        {
            Uri listenUri = new Uri("msmq://localhost/bill");
            Uri commandUri = new Uri("activemq://localhost/bob");
            Uri subUri = new Uri("msmq://localhost/mt_pubsub");

            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn("msmq://localhost/bill")
                .WithSharedSubscriptions<LocalSubscriptionCache>("msmq://localhost/mt_pubsub")
                //.WithSharedSubscriptions(Via.DistributedCache("tcp://192.168.0.1"))
                .CommunicatesOn<LoopbackEndpoint>()
                .UsingForSerialization<JsonBodyFormatter>()
                .Validate();
            //.ActivateHeartBeat();

            Assert.AreEqual(listenUri, busOptions.ListensOn);
            Assert.AreEqual(subUri, busOptions.Subcriptions.Address);
            Assert.AreEqual(typeof(LocalSubscriptionCache), busOptions.Subcriptions.SubscriptionStore); //how to best handle this?
            Assert.AreEqual(typeof(JsonBodyFormatter), busOptions.Serialization.Serializer); //how to handle this?
            Assert.AreEqual(false, busOptions.IsACompetingConsumer);
            //transports
        }

        [Test]
        public void As_a_commandee()
        {
            Uri listenUri = new Uri("msmq://localhost/bill");
            Uri commandUri = new Uri("activemq://localhost/bob");

            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn("msmq://localhost/bill")
                .ReceivesCommandsOn(commandUri)
                .CommunicatesOn<LoopbackEndpoint>() //TODO: what should the default be?
                .Validate();

            Assert.AreEqual(listenUri, busOptions.ListensOn);
            Assert.AreEqual(commandUri, busOptions.CommandedOn);
        }
    }
}