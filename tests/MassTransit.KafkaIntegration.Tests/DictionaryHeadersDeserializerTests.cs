namespace MassTransit.KafkaIntegration.Tests
{
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
            Assert.IsTrue(result.TryGetHeader("EmptyValue", out var emptyValue));
            Assert.IsNull(emptyValue);
        }
    }
}
