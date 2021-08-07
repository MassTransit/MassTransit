namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Context;
    using MassTransit.Serialization;
    using MassTransit.Transports.InMemory.Contexts;
    using MassTransit.Transports.InMemory.Fabric;
    using Messages;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    [Explicit]
    public class Serializer_performance :
        SerializationTest
    {
        [Test]
        public void Just_how_fast_are_you()
        {
            var message = new SerializationTestMessage
            {
                DecimalValue = 123.45m,
                LongValue = 098123213,
                BoolValue = true,
                ByteValue = 127,
                IntValue = 123,
                DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
                TimeSpanValue = TimeSpan.FromSeconds(30),
                GuidValue = Guid.NewGuid(),
                StringValue = "Chris's Sample Code",
                DoubleValue = 1823.172
            };

            var sendContext = new MessageSendContext<SerializationTestMessage>(message);
            ReceiveContext receiveContext = null;
            //warm it up
            for (var i = 0; i < 10; i++)
            {
                byte[] data = Serialize(sendContext);

                var transportMessage = new InMemoryTransportMessage(Guid.NewGuid(), data, Serializer.ContentType.MediaType,
                    TypeMetadataCache<SerializationTestMessage>.ShortName);
                receiveContext = new InMemoryReceiveContext(transportMessage, TestConsumeContext.GetContext());

                Deserialize<SerializationTestMessage>(receiveContext);
            }

            var timer = Stopwatch.StartNew();

            const int iterations = 50000;

            for (var i = 0; i < iterations; i++)
                Serialize(sendContext);

            timer.Stop();

            var perSecond = iterations * 1000 / timer.ElapsedMilliseconds;

            Console.WriteLine("Serialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);


            timer = Stopwatch.StartNew();

            for (var i = 0; i < 50000; i++)
                Deserialize<SerializationTestMessage>(receiveContext);

            timer.Stop();

            perSecond = iterations * 1000 / timer.ElapsedMilliseconds;

            Console.WriteLine("Deserialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
        }

        public Serializer_performance(Type serializerType)
            : base(serializerType)
        {
        }

        protected byte[] Serialize<T>(SendContext<T> sendContext)
            where T : class
        {
            using (var output = new MemoryStream())
            {
                Serializer.Serialize(output, sendContext);

                return output.ToArray();
            }
        }

        protected ConsumeContext<T> Deserialize<T>(ReceiveContext receiveContext)
            where T : class
        {
            var consumeContext = Deserializer.Deserialize(receiveContext);

            consumeContext.TryGetMessage(out ConsumeContext<T> messageContext);

            return messageContext;
        }
    }
}
