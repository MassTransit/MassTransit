namespace MassTransit.Tests.Serialization
{
    using System;
    using MassTransit.Serialization;
    using MessageData;
    using NUnit.Framework;

    
    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    public class Serialization_a_message_data_property :
        SerializationTest
    {
        public class SampleMessage
        {
            public MessageData<string> Value { get; set; }
        }

        public Serialization_a_message_data_property(Type serializerType)
            : base(serializerType)
        {
        }

        [Test]
        public void Should_handle_a_string_null()
        {
            var obj = new SampleMessage { Value = new StringMessageData(new Uri("urn:msgdata:098098120983123"), "Hello, World!") };

            Serialize(obj);
        }
    }
}
