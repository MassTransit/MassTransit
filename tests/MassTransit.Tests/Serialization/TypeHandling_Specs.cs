namespace MassTransit.Tests.Serialization
{
    using System.IO;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class Should_ignore_the_type_attribute
    {
        [Test]
        public void When_deserializing_a_json_body_with_types()
        {
            var serializer = NewtonsoftJsonMessageSerializer.Deserializer;

            var json = "{\"$type\":\"Command.TestCommand, TestDeserializationWithDummyClasses\",\"Id\":1,\"Name\":\"bob\"}";
            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var cmd = serializer.Deserialize<ITestCommand>(jsonReader);
            }
        }


        public interface ITestCommand
        {
            int Id { get; }
            string Name { get; }
        }


        public class TestCommand : ITestCommand
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
