namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Deserializing_an_enumerable_property :
        SerializationTest
    {
        [Test]
        public void Should_deserialize_the_enumerable_type()
        {
            EnumerableMessageType message = new EnumerableMessageTypeImpl {Items = new[] {new MessageItem {Value = "Frank"}, new MessageItem {Value = "Mary"}}};

            var result = SerializeAndReturn(message);

            result.Items.Count().ShouldBe(message.Items.Count());
        }

        public Deserializing_an_enumerable_property(Type serializerType)
            : base(serializerType)
        {
        }
    }


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Using_the_serializer_for_arrays :
        SerializationTest
    {
        [Test]
        public void Should_deserialize_multi_dimensional_arrays()
        {
            var message = new DoubleTheMessage {Values = new[,] {{1, 2}, {3, 4}, {5, 6}}};

            var result = SerializeAndReturn(message);

            Assert.That(result.Values, Is.Not.Null);
            Assert.That(result.Values.Length, Is.EqualTo(6), "Length");
            Assert.That(result.Values[1, 1], Is.EqualTo(4), "Value");
        }

        public Using_the_serializer_for_arrays(Type serializerType)
            : base(serializerType)
        {
        }
    }


    public class DoubleTheMessage
    {
        public int[,] Values { get; set; }
    }


    public interface EnumerableMessageType
    {
        IEnumerable<MessageItem> Items { get; }
    }


    class EnumerableMessageTypeImpl : EnumerableMessageType
    {
        public IEnumerable<MessageItem> Items { get; set; }
    }


    public class MessageItem
    {
        public string Value { get; set; }
    }
}
