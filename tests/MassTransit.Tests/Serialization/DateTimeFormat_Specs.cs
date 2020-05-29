namespace MassTransit.Tests.Serialization
{
    using System.IO;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class DateTimeFormat_Specs
    {
        [Test]
        public void Test()
        {
            var message = @"{ IsoDate: ""1994-11-05T13:15:30Z"" }";

            using (var sw = new StringReader(message))
            using (var jsonReader = new JsonTextReader(sw))
            {
                var obj = JsonMessageSerializer.Deserializer.Deserialize(jsonReader, typeof(MessageWithIsoDate));
                var msg = obj as MessageWithIsoDate;

                Assert.That(msg.IsoDate, Is.EqualTo("1994-11-05T13:15:30Z"));
            }
        }


        class MessageWithIsoDate
        {
            public string IsoDate { get; set; }
        }
    }
}
