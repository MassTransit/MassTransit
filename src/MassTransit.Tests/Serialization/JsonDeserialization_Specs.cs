using MassTransit.Serialization;
using MassTransit.Tests.Messages;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Tests.Serialization
{
    public class When_deserializing_messages_with_json_dot_net
    {
        JsonSerializer _deserializer;
        JsonSerializer _serializer;

        [SetUp]
        public void Deserializing_messages_with_json_dot_net()
        {
            _serializer = JsonMessageSerializer.Serializer;
            _deserializer = JsonMessageSerializer.Deserializer;
        }

        [Test]
        public void Should_be_able_to_deserialize_datetimeformatinfo()
        {
            DateTimeFormatInfo message = DateTimeFormatInfo.CurrentInfo;
            DateTimeFormatInfo result;

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    _serializer.Serialize(jsonWriter, message);

                    writer.Flush();

                    memoryStream.Position = 0;

                    using (var reader = new StreamReader(memoryStream))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        result = _deserializer.Deserialize<DateTimeFormatInfo>(jsonReader);
                    }

                    result.ShouldBeOfType<DateTimeFormatInfo>();
                }
            }
        }

        [Test]
        public void Should_be_able_to_deserialize_numberformatinfo()
        {
            NumberFormatInfo message = NumberFormatInfo.CurrentInfo;
            NumberFormatInfo result;

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    _serializer.Serialize(jsonWriter, message);

                    writer.Flush();

                    memoryStream.Position = 0;

                    using (var reader = new StreamReader(memoryStream))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        result = _deserializer.Deserialize<NumberFormatInfo>(jsonReader);
                    }

                    result.ShouldBeOfType<NumberFormatInfo>();
                }
            }
        }
    }
}
