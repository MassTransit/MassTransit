namespace MassTransit.ServiceBus.Tests.Configuration
{
    using System;
    using MassTransit.ServiceBus.Configuration;
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
            Uri commandUri = new Uri("activemq://localhost/bob");
            Uri subUri = new Uri("msmq://localhost/mt_pubsub");

            var busOptions = BusBuilder.WithName("funk_bus")
                .ListensOn("msmq://localhost/bill")
                .ReceivesCommandsOn("activemq://localhost/bob")
                .WithSharedSubscriptions(Via.SubscriptionService("msmq://localhost/mt_pubsub"))
                //.WithSharedSubscriptions(Via.DistributedCache("tcp://192.168.0.1"))
                .CommunicatesOn<LoopbackEndpoint>()
                .AsACompetingConsumer()
                .Using(Serializers.Binary)
                .Validate();
            //.ActivateHeartBeat();

            Assert.AreEqual(listenUri, busOptions.ListensOn);
            Assert.AreEqual(commandUri, busOptions.CommandedOn);
            Assert.AreEqual(subUri, busOptions.Subcriptions.Address);
            //Assert.AreEqual(null, busOptions.Subcriptions.SubscriptionStore);
//            Assert.AreEqual(null, busOptions.Serialization.Serializer);
//            Assert.AreEqual(null, busOptions.IsACompetingConsumer);
            //transports
        }
    }
}