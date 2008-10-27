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
        readonly Uri _listenUri = new Uri("msmq://localhost/bill");
        readonly Uri _commandUri = new Uri("activemq://localhost/bob");
        readonly Uri _subUri = new Uri("msmq://localhost/mt_pubsub");

        [SetUp]
        public void SetUp()
        {
            IObjectBuilder builder = null;
            BusBuilder.SetObjectBuilder(builder);
        }

        [Test]
        public void Minimal_Setup()
        {

            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn(_listenUri)
                .CommunicatesOver<LoopbackEndpoint>() //TODO: what should the default be?
                .Validate();

            Assert.AreEqual(_listenUri, busOptions.ListensOn);
            Assert.AreEqual(EndpointResolver.Null.Uri, busOptions.CommandedOn);
            Assert.AreEqual(typeof(BinaryBodyFormatter), busOptions.Serialization.Serializer); //how to handle this?
            Assert.AreEqual(false, busOptions.IsACompetingConsumer);
            Assert.IsNotNull(busOptions.Subcriptions);
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
            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn(_listenUri)
                .SharesSubscriptionsVia<LocalSubscriptionCache>("msmq://localhost/mt_pubsub")
                .CommunicatesOver<LoopbackEndpoint>()
                .UsingForSerialization<JsonBodyFormatter>()
                .Validate();

            Assert.AreEqual(_listenUri, busOptions.ListensOn);
            Assert.AreEqual(_subUri, busOptions.Subcriptions.Address);
            Assert.AreEqual(typeof(LocalSubscriptionCache), busOptions.Subcriptions.SubscriptionStore); //how to best handle this?
            Assert.AreEqual(typeof(JsonBodyFormatter), busOptions.Serialization.Serializer); //how to handle this?
            Assert.AreEqual(false, busOptions.IsACompetingConsumer);
        }

        [Test]
        public void Turn_on_heartbeat()
        {
//            var busOptions = BusBuilder.WithName("funk_bus")
//                .ActivateHeartBeat();
        }
        [Test]
        public void As_a_commandee()
        {
            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn(_listenUri)
                .ReceivesCommandsOn(_commandUri)
                .CommunicatesOver<LoopbackEndpoint>() //TODO: what should the default be?
                .Validate();

            Assert.AreEqual(_listenUri, busOptions.ListensOn);
            Assert.AreEqual(_commandUri, busOptions.CommandedOn);
        }
    }
}