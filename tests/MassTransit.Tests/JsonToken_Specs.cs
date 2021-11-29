namespace MassTransit.Tests
{
    using System.IO;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_JsonToken_to_convert_types
    {
        [Test]
        public void Should_support_int()
        {
            using (var reader = new StringReader("27"))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var value = NewtonsoftJsonMessageSerializer.Deserializer.Deserialize<int>(jsonReader);

                Assert.AreEqual(27, value);
            }
        }
    }
}
