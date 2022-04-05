namespace MassTransit.Tests.Serialization
{
    namespace AttributeSerialization
    {
        using System;
        using System.Text.Json;
        using System.Text.Json.Serialization;
        using MassTransit.Serialization;
        using NUnit.Framework;


        public class SillyValueJsonConverter :
            JsonConverter<int>
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return -1;
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                writer.WriteStringValue("1");
            }
        }


        public interface SillyMessage
        {
            [JsonConverter(typeof(SillyValueJsonConverter))]
            public int Value { get; set; }
        }


        public class Serializing_an_interface_with_a_property_attribute
        {
            [Test]
            public void Should_copy_the_property_to_the_interface()
            {
                var messageSerializer = new SystemTextJsonMessageSerializer();
                var value = messageSerializer.DeserializeObject<SillyMessage>("{\"Value\": 10}");

                Assert.That(value?.Value, Is.EqualTo(-1));
            }
        }
    }
}
