namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using MassTransit.Serialization;
    using NUnit.Framework;


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
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

            var obj = new SampleMessage {Value = await repository.PutString(new string('*', MessageDataDefaults.Threshold + 100))};

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
