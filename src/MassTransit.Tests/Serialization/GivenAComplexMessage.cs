namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Context;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Serialization;
    using Messages;
    using NUnit.Framework;

    public abstract class GivenAComplexMessage<TSerializer> where TSerializer : IMessageSerializer, new()
    {
        public SerializationTestMessage Message { get; private set; }

        public GivenAComplexMessage()
        {

            Message = new SerializationTestMessage
                {
                    DecimalValue = 123.45m,
                    LongValue = 098123213,
                    BoolValue = true,
                    ByteValue = 127,
                    IntValue = 123,
                    DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
                    TimeSpanValue = 30.Seconds(),
                    GuidValue = Guid.NewGuid(),
                    StringValue = "Chris's Sample Code",
                    DoubleValue = 1823.172,
                    MaybeMoney = 567.89m,
                };
        }

        [Test]
        public void ShouldWork()
        {
            byte[] serializedMessageData;

            var serializer = new TSerializer();

            using (var output = new MemoryStream())
            {
                serializer.Serialize(output, new SendContext<SerializationTestMessage>(Message));

                serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            using (var input = new MemoryStream(serializedMessageData))
            {
            	var receiveContext = ReceiveContext.FromBodyStream(input);
            	serializer.Deserialize(receiveContext);

            	IConsumeContext<SerializationTestMessage> context;
            	receiveContext.TryGetContext<SerializationTestMessage>(out context).ShouldBeTrue();

            	context.ShouldNotBeNull();

            	context.Message.ShouldEqual(Message);
            }
        }
    }


    [TestFixture]
    public class WhenUsingTheCustomXmlOnComplexMessage :
        GivenAComplexMessage<XmlMessageSerializer>
    {

    }

    [TestFixture]
    public class WhenUsingTheBinaryOnComplexMessage :
        GivenAComplexMessage<BinaryMessageSerializer>
    {

    }

    [TestFixture]
    public class WhenUsingJsonOnComplexMessage :
        GivenAComplexMessage<JsonMessageSerializer>
    {

    }

//	[TestFixture]
//    public class WhenUsingBsonOnComplexMessage :
//        GivenAComplexMessage<BsonMessageSerializer>
//    {
//
//    }
}