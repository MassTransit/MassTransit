namespace MassTransit.LegacySupport.Tests
{
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using SerializationCustomization;
    using Subscriptions;
    using Subscriptions.Messages;

    [TestFixture]
    public class TestSerialization
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Old = new BinaryFormatter();
            NewReader = new BinaryFormatter();
            NewWriter = new BinaryFormatter();

            //smelly
            var map = new LegacyMessageMappings();
            var b = new LegacyBinder();
            foreach (MessageMap pair in map)
            {
                b.AddMap(pair.FullTypeName, pair.NewType);
            }

            NewReader.Binder = b;

            var ss = new LegacySurrogateSelector();
            ss.AddSurrogate(new LegacySurrogate<OldCancelSubscriptionUpdates>("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CancelSubscriptionUpdates"));
            ss.AddSurrogate(new LegacySurrogate<OldRemoveSubscription>("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription"));
            ss.AddSurrogate(new LegacySurrogate<OldAddSubscription>("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription"));
            ss.AddSurrogate(new LegacySurrogate<Subscription>("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Subscription"));
            NewWriter.SurrogateSelector = ss;
        }

        public BinaryFormatter NewReader { get; private set; }
        public BinaryFormatter NewWriter { get; private set; }
        public BinaryFormatter Old { get; private set; }
    }
}