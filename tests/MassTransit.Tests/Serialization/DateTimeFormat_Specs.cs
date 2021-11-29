namespace MassTransit.Tests.Serialization
{
    using System.IO;
    using System.Text;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class DateTimeFormat_Specs
    {
        [Test]
        public void Using_json_net()
        {
            var message = @"{ ""IsoDate"": ""1994-11-05T13:15:30Z"" }";

            using var sw = new StringReader(message);
            using var jsonReader = new JsonTextReader(sw);

            var obj = NewtonsoftJsonMessageSerializer.Deserializer.Deserialize(jsonReader, typeof(MessageWithIsoDate));
            var msg = obj as MessageWithIsoDate;

            Assert.That(msg.IsoDate, Is.EqualTo("1994-11-05T13:15:30Z"));
        }

        [Test]
        public void Using_system_text_json()
        {
            var message = @"{ ""IsoDate"": ""1994-11-05T13:15:30Z"" }";

            var msg = System.Text.Json.JsonSerializer.Deserialize<MessageWithIsoDate>(Encoding.UTF8.GetBytes(message), SystemTextJsonMessageSerializer.Options);

            Assert.That(msg.IsoDate, Is.EqualTo("1994-11-05T13:15:30Z"));
        }


        class MessageWithIsoDate
        {
            public string IsoDate { get; set; }
        }
    }
}
