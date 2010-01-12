namespace MassTransit.LegacySupport.Tests
{
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
            var ss = new LegacySurrogateSelector();
            foreach (MessageMap pair in map)
            {
                b.AddMap(pair.FullTypeName, pair.NewType);
                ss.AddSurrogate(new LegacySurrogate(pair.AssemblyName, pair.FullTypeName, pair.NewType));
            }

            NewReader.Binder = b;
            NewWriter.SurrogateSelector = ss;
        }

        public BinaryFormatter NewReader { get; private set; }
        public BinaryFormatter NewWriter { get; private set; }
        public BinaryFormatter Old { get; private set; }
    }
}