using MassTransit.KafkaIntegration.Serializers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.KafkaIntegration.Tests
{
    public class DictionaryHeadersSerializeTests
    {
        [Test]
        public async Task Should_Deserialize_with_empty_header_value()
        {
            Confluent.Kafka.Headers headers = new Confluent.Kafka.Headers();
            headers.Add(new Confluent.Kafka.Header("EmptyValue", null));
            var result = DictionaryHeadersSerialize.Deserializer.Deserialize(headers);
            Assert.IsTrue(result.TryGetHeader("EmptyValue", out object? emptyValue));
            Assert.IsNull(emptyValue);
        }
    }
}
