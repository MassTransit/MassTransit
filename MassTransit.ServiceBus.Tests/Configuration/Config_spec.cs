namespace MassTransit.ServiceBus.Tests.Configuration
{
    using System;
    using NUnit.Framework;
    using Transports;

    [TestFixture]
    public class Config_spec :
        Specification
    {
        [Test]
        public void Minimal_Setup()
        {
            BusBuilder.WithName("funk_bus")
                .ListensOn("msmq://localhost/bill")
                .ReceivesCommandsOn("activemq://localhost/bob")
                .WithSharedSubscriptions(Via.SubscriptionService("msmq://localhost/mt_pubsub"))
                .WithSharedSubscriptions(Via.DistributedCache("192.168.0.1"))
                .CommunicatesOn<LoopbackEndpoint>()
                .AsACompetingConsumer()
                .Using(Serializers.Binary);
        }
    }

    public static class BusBuilder
    {
        public static ConfigureTheBus WithName(string name)
        {
            return new ConfigureTheBus();
        }
    }
    
    public class ConfigureTheBus
    {
        public ConfigureTheBus ListensOn(string uri)
        {
            return this;
        }

        public ConfigureTheBus ReceivesCommandsOn(string uri)
        {
            return this;
        }

        public ConfigureTheBus WithSharedSubscriptions(object subscriptionStore)
        {
            return this;
        }

        public ConfigureTheBus CommunicatesOn<T>()
        {
            return this;
        }

        public ConfigureTheBus AsACompetingConsumer()
        {
            return this;
        }

        public ConfigureTheBus Using(object serializer)
        {
            return this;
        }
    }
    public static class Serializers
    {
        public static SerializationOptions Binary
        {
            get { return new SerializationOptions(); }
        }

        public static SerializationOptions Xml
        {
            get { return new SerializationOptions(); }
        }

        public static SerializationOptions Custom<T>()
        {
            return new SerializationOptions();
        }
    }
    public static class Via
    {
        public static SubscriptionOptions SubscriptionService(string uri)
        {
            return new SubscriptionOptions();
        }

        public static SubscriptionOptions DistributedCache(string uri)
        {
            return new SubscriptionOptions();
        }

        public static SubscriptionOptions Custom<T>(string uri)
        {
            return new SubscriptionOptions();
        }
    }

    //state pattern?
    public class BusOptions
    {
        public Uri ListensOn { get; set; }
        public Uri CommandedOn { get; set; }
        public bool IsACompetingConsumer { get; set; }
        public SerializationOptions Serialization { get; set; }
        public SubscriptionOptions Subcriptions { get; set; }
        //transportation
        public string Name { get; set; }
        public object ResolvesDependenciesFrom { get; set; }
    }
    public class SerializationOptions
    {
    }
    public class SubscriptionOptions
    {
        public string Address;
        public object TypeOfSubscriptionStore;
    }
}