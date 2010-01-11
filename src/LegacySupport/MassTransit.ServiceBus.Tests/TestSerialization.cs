namespace MassTransit.LegacySupport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using SerializationCustomization;
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
            NewWriter.SurrogateSelector = ss;
        }

        public BinaryFormatter NewReader { get; private set; }
        public BinaryFormatter NewWriter { get; private set; }
        public BinaryFormatter Old { get; private set; }
    }


    [TestFixture]
    public class SerializeCancelUpdates :
        TestSerialization
    {
        private string _pathToFile = @".\OldSerializedMessages\CancelSubscriptionUpdates.txt";

        [Test]
        public void OldToNew()
        {
            OldCancelSubscriptionUpdates oldMsg;
            using (var str = File.OpenRead(_pathToFile))
            {
                oldMsg = (OldCancelSubscriptionUpdates)NewReader.Deserialize(str);
            }
            Assert.AreEqual(new Uri("http://bob/phil"),oldMsg.RequestingUri);
        }

        [Test]
        public void NewToOld()
        {
            var oldMsg = new OldCancelSubscriptionUpdates(new Uri("http://bob/phil"));
            using (var newStream = new MemoryStream())
            {
                NewWriter.Serialize(newStream, oldMsg);

                newStream.Position = 0;

                using (var oldStream = new MemoryStream())
                {
                    using (var str = File.OpenRead(_pathToFile))
                    {
                        var buff = new byte[str.Length];
                        str.Read(buff, 0, buff.Length);
                        oldStream.Write(buff, 0, buff.Length);
                    }

                    StreamAssert.AreEqual(oldStream, newStream);
                }
            }
        }

        [Test]
        public void NAME()
        {
            var e = (char)14;
            var a = (char)13;

            Console.WriteLine("'{0}'", e);
            Console.WriteLine("'{0}'", a);

        }


    }
}