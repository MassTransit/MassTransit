namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using MassTransit.Serialization;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using NUnit.Framework;


    public class When_serializing_messages_with_json_dot_net
    {
        static readonly Type _proxyType = TypeMetadataCache<MessageA>.ImplementationType;
        string _body;
        JsonSerializer _deserializer;
        Envelope _envelope;
        TestMessage _message;
        JsonSerializer _serializer;
        XDocument _xml;

        [SetUp]
        public void Serializing_messages_with_json_dot_net()
        {
            _message = new TestMessage
            {
                Name = "Joe",
                Details = new List<TestMessageDetail>
                {
                    new TestMessageDetail
                    {
                        Item = "A",
                        Value = 27.5d
                    },
                    new TestMessageDetail
                    {
                        Item = "B",
                        Value = 13.5d
                    }
                },
                EnumDetails = new List<TestMessageDetail>
                {
                    new TestMessageDetail
                    {
                        Item = "1",
                        Value = 42.0d
                    }
                }
            };

            _envelope = new Envelope { Message = _message };

            _envelope.MessageType.Add(_message.GetType().ToMessageName());
            _envelope.MessageType.Add(typeof(MessageA).ToMessageName());
            _envelope.MessageType.Add(typeof(MessageB).ToMessageName());

            _envelope.Headers.Add("Simple", "Value");
            _envelope.Host = new BusHostInfo(true);

            _serializer = NewtonsoftJsonMessageSerializer.Serializer;
            _deserializer = NewtonsoftJsonMessageSerializer.Deserializer;

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                _serializer.Serialize(jsonWriter, _envelope);

                writer.Flush();
                _body = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            _xml = JsonConvert.DeserializeXNode(_body, "envelope");
        }

        [Test]
        [Explicit]
        public void Show_me_the_message()
        {
            TestContext.Out.WriteLine(_body);
            TestContext.Out.WriteLine(_xml.ToString());
        }

        [Test]
        public void Convert_using_json_deserializer()
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_body), false);
            using var reader = new StreamReader(memoryStream);
            using var jsonReader = new JsonTextReader(reader);

            var result = _deserializer.Deserialize<JsonMessageContext<TestMessage>>(jsonReader);

            Assert.That(result.Message.Name, Is.EqualTo("Joe"));
        }

        [Test]
        public void Should_be_able_to_resurrect_the_message()
        {
            Envelope result;
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_body), false))
            using (var reader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                result = _deserializer.Deserialize<Envelope>(jsonReader);
            }

            Assert.That(result.MessageType, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(result.MessageType[0], Is.EqualTo(typeof(TestMessage).ToMessageName()));
                Assert.That(result.MessageType[1], Is.EqualTo(typeof(MessageA).ToMessageName()));
                Assert.That(result.MessageType[2], Is.EqualTo(typeof(MessageB).ToMessageName()));
                Assert.That(result.Headers, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Should_be_able_to_suck_out_an_interface()
        {
            Envelope result;
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_body), false))
            using (var reader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                result = _deserializer.Deserialize<Envelope>(jsonReader);
            }

            using (var jsonReader = new JTokenReader(result.Message as JToken))
            {
                var message = (MessageA)Activator.CreateInstance(_proxyType);

                _serializer.Populate(jsonReader, message);

                Assert.That(message.Name, Is.EqualTo("Joe"));
            }
        }

        [Test]
        public void Should_be_able_to_suck_out_an_object()
        {
            Envelope result;
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_body), false))
            using (var reader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                result = _deserializer.Deserialize<Envelope>(jsonReader);
            }

            using (var jsonReader = new JTokenReader(result.Message as JToken))
            {
                var message = (TestMessage)_serializer.Deserialize(jsonReader, typeof(TestMessage));

                Assert.Multiple(() =>
                {
                    Assert.That(message.Name, Is.EqualTo("Joe"));
                    Assert.That(message.Details, Has.Count.EqualTo(2));

                    Assert.That(message.EnumDetails.Count(), Is.EqualTo(1));
                });
            }
        }

        [Test]
        public void Should_be_able_to_resurrect_the_message_from_xml()
        {
            var document = XDocument.Parse(_xml.ToString());
            Trace.WriteLine(_xml.ToString());
            var body = JsonConvert.SerializeXNode(document, Formatting.None, true);
            Trace.WriteLine(body);
            var result = JsonConvert.DeserializeObject<Envelope>(body,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            Assert.That(result.MessageType, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(result.MessageType[0], Is.EqualTo(typeof(TestMessage).ToMessageName()));
                Assert.That(result.MessageType[1], Is.EqualTo(typeof(MessageA).ToMessageName()));
                Assert.That(result.MessageType[2], Is.EqualTo(typeof(MessageB).ToMessageName()));
                Assert.That(result.Headers, Has.Count.EqualTo(1));
            });
        }

        [Test]
        [Explicit]
        public void Serialization_speed()
        {
            //warm it up
            for (var i = 0; i < 10; i++)
            {
                DoSerialize();
                DoDeserialize();
            }

            var timer = Stopwatch.StartNew();

            const int iterations = 50000;

            for (var i = 0; i < iterations; i++)
                DoSerialize();

            timer.Stop();

            var perSecond = iterations * 1000 / timer.ElapsedMilliseconds;

            var msg = string.Format("Serialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
            Trace.WriteLine(msg);

            timer = Stopwatch.StartNew();

            for (var i = 0; i < 50000; i++)
                DoDeserialize();

            timer.Stop();

            perSecond = iterations * 1000 / timer.ElapsedMilliseconds;

            msg = string.Format("Deserialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
            Trace.WriteLine(msg);
        }

        void DoSerialize()
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                _serializer.Serialize(jsonWriter, _envelope);

                writer.Flush();
                _body = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        void DoDeserialize()
        {
            Envelope result;
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_body), false))
            using (var reader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                result = _deserializer.Deserialize<Envelope>(jsonReader);
            }

            using (var jsonReader = new JTokenReader(result.Message as JToken))
            {
                var message = (MessageA)Activator.CreateInstance(_proxyType);

                _serializer.Populate(jsonReader, message);
            }
        }

        public static object Convert(string s)
        {
            var obj = JsonConvert.DeserializeObject(s);
            if (obj is string)
                return obj as string;

            return ConvertJson((JToken)obj);
        }

        static object ConvertJson(JToken token)
        {
            var value = token as JValue;
            if (value != null)
                return value.Value;

            if (token is JObject)
            {
                IDictionary<string, object> expando = new Dictionary<string, object>();

                (from childToken in token where childToken is JProperty select childToken as JProperty).ToList().ForEach(
                    property =>
                    {
                        expando.Add(property.Name, ConvertJson(property.Value));
                    });
                return expando;
            }

            if (token is JArray)
                return token.Select(ConvertJson).ToList();

            throw new ArgumentException(string.Format("VisitUnknownFilter token type '{0}'", token.GetType()), "token");
        }


        class Envelope
        {
            public Envelope()
            {
                Headers = new Dictionary<string, string>();
                MessageType = new List<string>();
            }

            public IDictionary<string, string> Headers { get; set; }
            public object Message { get; set; }
            public IList<string> MessageType { get; set; }
            public BusHostInfo Host { get; set; }
        }


        public interface MessageA
        {
            string Name { get; }
        }


        public interface MessageB
        {
            string Address { get; }
            string Name { get; }
        }


        class TestMessage :
            MessageA
        {
            public string Address { get; set; }
            public IList<TestMessageDetail> Details { get; set; }
            public int Level { get; set; }
            public IEnumerable<TestMessageDetail> EnumDetails { get; set; }
            public string Name { get; set; }
        }


        class TestMessageDetail
        {
            public string Item { get; set; }
            public double Value { get; set; }
        }
    }


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
    public class Using_JsonConverterAttribute_on_a_class :
        SerializationTest
    {
        [Test]
        public void Should_use_converter_for_deserialization()
        {
            var obj = new MessageB { Value = "Joe" };

            var result = SerializeAndReturn(obj);

            Assert.That(result.Value, Is.EqualTo("Monster"));
        }

        [Test]
        public void Should_use_converter_for_serialization()
        {
            var obj = new MessageA { Value = "Joe" };

            var result = SerializeAndReturn(obj);

            Assert.That(result.Value, Is.EqualTo("Monster"));
        }


        public class ModifyWhenSerializingConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("value");
                writer.WriteValue("Monster");

                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                reader.Read();
                Assert.That(reader.Value, Is.EqualTo("value"));
                reader.Read();
                var value = (string)reader.Value;
                return new MessageA { Value = value };
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(MessageA).IsAssignableFrom(objectType);
            }
        }


        public class ModifyWhenDeserializingConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var testMessage = (MessageB)value;
                writer.WriteStartObject();

                writer.WritePropertyName("value");
                writer.WriteValue(testMessage.Value);

                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new MessageB { Value = "Monster" };
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(MessageB).IsAssignableFrom(objectType);
            }
        }


        [JsonConverter(typeof(ModifyWhenSerializingConverter))]
        public class MessageA
        {
            public string Value { get; set; }
        }


        [JsonConverter(typeof(ModifyWhenDeserializingConverter))]
        public class MessageB
        {
            public string Value { get; set; }
        }


        public Using_JsonConverterAttribute_on_a_class(Type serializerType)
            : base(serializerType)
        {
        }
    }


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
    public class Using_JsonConverterAttribute_on_a_property :
        SerializationTest
    {
        [Test]
        public void Should_use_converter_for_deserialization()
        {
            var obj = new SimpleMessage { ValueB = "Joe" };

            var result = SerializeAndReturn(obj);

            Assert.That(result.ValueB, Is.EqualTo("Monster"));
        }

        [Test]
        public void Should_use_converter_for_serialization()
        {
            var obj = new SimpleMessage { ValueA = "Joe" };

            var result = SerializeAndReturn(obj);

            Assert.That(result.ValueA, Is.EqualTo("Monster"));
        }


        public class ModifyWhenSerializingConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue("Monster");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return reader.Value;
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(string).IsAssignableFrom(objectType);
            }
        }


        public class ModifyWhenDeserializingConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue((string)value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return "Monster";
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(string).IsAssignableFrom(objectType);
            }
        }


        public class SimpleMessage
        {
            [JsonConverter(typeof(ModifyWhenSerializingConverter))]
            public string ValueA { get; set; }

            [JsonConverter(typeof(ModifyWhenDeserializingConverter))]
            public string ValueB { get; set; }
        }


        public Using_JsonConverterAttribute_on_a_property(Type serializerType)
            : base(serializerType)
        {
        }
    }


    public class When_serializing_decimals
    {
        [Test]
        public void Should_deserialize_correctly()
        {
            // arrange
            var message = new MessageA { Decimal = decimal.MaxValue };

            // act, assert
            var serializedMessage = JsonConvert.SerializeObject(message, NewtonsoftJsonMessageSerializer.SerializerSettings);
            Assert.That(serializedMessage, Is.Not.Null);

            var deserializedMessage = JsonConvert.DeserializeObject<MessageA>(serializedMessage, NewtonsoftJsonMessageSerializer.DeserializerSettings);
            Assert.That(deserializedMessage, Is.Not.Null);
            Assert.That(deserializedMessage.Decimal, Is.EqualTo(message.Decimal));
        }

        [Test]
        public void Should_deserialize_correctly_with_stj()
        {
            // arrange
            var message = new MessageA { Decimal = decimal.MaxValue };

            // act, assert
            var serializedMessage = JsonConvert.SerializeObject(message, NewtonsoftJsonMessageSerializer.SerializerSettings);
            Assert.That(serializedMessage, Is.Not.Null);

            var deserializedMessage = System.Text.Json.JsonSerializer.Deserialize<MessageA>(serializedMessage, SystemTextJsonMessageSerializer.Options);
            Assert.That(deserializedMessage, Is.Not.Null);
            Assert.That(deserializedMessage.Decimal, Is.EqualTo(message.Decimal));
        }


        class MessageA
        {
            public decimal Decimal { get; set; }
        }
    }
}
