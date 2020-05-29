namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using MassTransit.Serialization;
    using NUnit.Framework;


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Serialization_a_message_data_property :
        SerializationTest
    {
        [Test]
        public async Task Should_handle_a_string_null()
        {
            var repository = new InMemoryMessageDataRepository();

            var dataId = new InMemoryMessageDataId().Uri;

            var obj = new SampleMessage {Value = await repository.PutString("Hello, World!")};

            Serialize(obj);
        }


        public class SampleMessage
        {
            public MessageData<string> Value { get; set; }
        }


        public Serialization_a_message_data_property(Type serializerType)
            : base(serializerType)
        {
        }
    }
}
