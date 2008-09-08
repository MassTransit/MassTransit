namespace MassTransit.ServiceBus.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using MassTransit.ServiceBus.Internal;
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

    //facade classes
    public static class BusBuilder
    {
        public static ConfigureTheBus WithName(string name)
        {
            return new ConfigureTheBus(new BusOptions{Name = name});
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
            return SubscriptionService(new Uri(uri));
        }
        public static SubscriptionOptions SubscriptionService(Uri uri)
        {
            return new SubscriptionOptions {Address = uri, SubscriptionStore =  typeof(object)};
        }

        public static SubscriptionOptions DistributedCache(string uri)
        {
            return DistributedCache(new Uri(uri));
        }
        public static SubscriptionOptions DistributedCache(Uri uri)
        {
            return new SubscriptionOptions{Address = uri};
        }

        public static SubscriptionOptions Custom<T>(string uri)
        {
            return new SubscriptionOptions();
        }
    }

    //state pattern?
    public class ConfigureTheBus
    {
        private readonly BusOptions _options;
        private readonly IList<Uri> _registeredUris;

        public ConfigureTheBus(BusOptions options)
        {
            _registeredUris = new List<Uri>();
            _options = options;
        }

        public ConfigureTheBus ListensOn(string uri)
        {
            return ListensOn(new Uri(uri));
        }
        public ConfigureTheBus ListensOn(Uri uri)
        {
            _registeredUris.Add(uri);
            _options.ListensOn = uri;
            return this;
        }

        public ConfigureTheBus ReceivesCommandsOn(string uri)
        {
            return ReceivesCommandsOn(new Uri(uri));
        }
        public ConfigureTheBus ReceivesCommandsOn(Uri uri)
        {
            _registeredUris.Add(uri);
            _options.CommandedOn = uri;
            return this;
        }

        public ConfigureTheBus WithSharedSubscriptions(SubscriptionOptions subscriptionOptions)
        {
            _options.Subcriptions = subscriptionOptions;
            return this;
        }
        public ConfigureTheBus Using(SerializationOptions serializer)
        {
            _options.Serialization = serializer;
            return this;
        }
        public ConfigureTheBus AsACompetingConsumer()
        {
            _options.MarkAsCompetingConsumer();
            return this;
        }

        public ConfigureTheBus CommunicatesOn<T>() where T : IEndpoint
        {
            _options.RegisterTransport<T>();
            return this;
        }


        //last thing you call
        public BusOptions Validate()
        {
            //that all uris have transports
            //EndpointResolver.EnsureThatTransportsExist(_registeredUris);  
            

            //that the IoC is not null?
            //any custom options should be checked
            return _options;
        }

    }
    public class BusOptions
    {
        public Uri ListensOn { get; set; }
        public Uri CommandedOn { get; set; }
        public bool IsACompetingConsumer { get; private set; }
        public void MarkAsCompetingConsumer()
        {
            IsACompetingConsumer = true;
        }
        public SerializationOptions Serialization { get; set; }
        public SubscriptionOptions Subcriptions { get; set; }
        public void RegisterTransport<T>()
        {
            EndpointResolver.AddTransport(typeof (T));
        }
        public string Name { get; set; }
        public object ResolvesDependenciesFrom { get; set; }
    }
    public class SerializationOptions
    {
        public Type Serializer { get; set; }
    }
    public class SubscriptionOptions
    {
        public Uri Address { get; set;}
        public Type SubscriptionStore { get; set; }
    }
}