namespace MassTransit.KafkaIntegration.Tests
{
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using NUnit.Framework;
    using Serializers;


    public class DictionaryHeadersSerializeTests
    {
        [Test]
        public async Task Should_Deserialize_with_empty_header_value()
        {
            var headers = new Headers { new Header("EmptyValue", null) };
            var result = DictionaryHeadersSerialize.Deserializer.Deserialize(headers);
            Assert.Multiple(() =>
            {
                Assert.That(result.TryGetHeader("EmptyValue", out var emptyValue), Is.True);
                Assert.That(emptyValue, Is.Null);
            });
        }

        [Test]
        public async Task Should_Deserialize_with_duplicate_header_value()
        {
            var headers = new Headers
            {
                new Header("TestValue", null),
                new Header("TestValue", null)
            };

            var result = DictionaryHeadersSerialize.Deserializer.Deserialize(headers);
            Assert.Multiple(() =>
            {
                Assert.That(result.TryGetHeader("TestValue", out var emptyValue), Is.True);
                Assert.That(emptyValue, Is.Null);
            });
        }

        [Test]
        public async Task Should_not_throw_when_header_in_different_encoding()
        {
            var bytes = Encoding.Unicode.GetBytes("test");
            var headers = new Headers { new Header("BadValue", bytes) };
            var result = DictionaryHeadersSerialize.Deserializer.Deserialize(headers);
            Assert.Multiple(() =>
            {
                Assert.That(result.TryGetHeader("BadValue", out var value), Is.True);
                Assert.That(value, Is.EqualTo(Encoding.UTF8.GetString(bytes)));
            });
        }
    }
}
