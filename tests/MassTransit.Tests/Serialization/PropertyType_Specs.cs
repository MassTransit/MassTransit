namespace MassTransit.Tests.Serialization
{
    using System;
    using MassTransit.Serialization;
    using NUnit.Framework;


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    [TestFixture(typeof(MessagePackMessageSerializer))]
    public class Serializing_a_property_of_type_char :
        SerializationTest
    {
        [Test]
        public void Should_handle_a_missing_nullable_value()
        {
            var obj = new PropertyOfNullableChar();

            var result = SerializeAndReturn(obj);

            Assert.That(result.Value, Is.EqualTo(obj.Value));
        }

        [Test]
        public void Should_handle_a_present_nullable_value()
        {
            var obj = new PropertyOfNullableChar {Value = 'A'};

            var result = SerializeAndReturn(obj);

            Assert.That(result.Value, Is.EqualTo(obj.Value));
        }

        [Test]
        public void Should_handle_a_present_value()
        {
            var obj = new PropertyOfChar {Value = 'A'};

            var result = SerializeAndReturn(obj);

            Assert.That(result.Value, Is.EqualTo(obj.Value));
        }

        [Test]
        public void Should_handle_a_string_null()
        {
            var obj = new PropertyOfChar {Value = '\0'};

            var result = SerializeAndReturn(obj);

            Assert.That(result.Value, Is.EqualTo(obj.Value));
        }


        public class PropertyOfChar
        {
            public char Value { get; set; }
        }


        public class PropertyOfNullableChar
        {
            public char? Value { get; set; }
        }


        public Serializing_a_property_of_type_char(Type serializerType)
            : base(serializerType)
        {
        }
    }


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Serializing_a_string_with_an_escaped_character :
        SerializationTest
    {
        [Test]
        public void Should_handle_a_missing_nullable_value()
        {
            var obj = new SimpleMessage("");

            var result = SerializeAndReturn(obj);

            Assert.That(result.Body, Is.EqualTo(obj.Body));
        }


        public class SimpleMessage
        {
            public SimpleMessage(string body)
            {
                Body = body;
            }

            protected SimpleMessage()
            {
            }

            public string Body { get; private set; }
        }


        public Serializing_a_string_with_an_escaped_character(Type serializerType)
            : base(serializerType)
        {
        }
    }
}
