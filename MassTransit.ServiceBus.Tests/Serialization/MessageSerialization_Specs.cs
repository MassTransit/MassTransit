// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Tests.Serialization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class MessageSerialization_Specs
    {
        [Test]
        public void FIRST_TEST_NAME()
        {
            SerializationTestMessage message = new SerializationTestMessage
                {
                    Amount = 123.45m,
                    BigCount = 098123213,
                    Count = 123,
                    Created = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Name = "Chris",
                    Radians = 1823.172,
                };

            byte[] serializedMessageData;

            using (MemoryStream output = new MemoryStream())
            using (IMessageSerializer serializer = new XmlMessageSerializer())
            {
                serializer.Serialize(output, message);

                serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            using (MemoryStream input = new MemoryStream(serializedMessageData))
            {
                using (IMessageSerializer serializer = new XmlMessageSerializer())
                {
                    SerializationTestMessage receivedMessage = serializer.Deserialize<SerializationTestMessage>(input);

                    Assert.AreEqual(message, receivedMessage);
                }
            }
        }
    }

    public class XmlMessageSerializer :
        IMessageSerializer
    {
        public void Dispose()
        {
        }

        public void Serialize<T>(Stream output, T message)
        {
            XmlMessageEnvelope envelope = new XmlMessageEnvelope(message);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); 

            XmlSerializer _serializer = new XmlSerializer(typeof (XmlMessageEnvelope), new[] {typeof (T)});

            _serializer.Serialize(output, envelope);
        }

        public T Deserialize<T>(Stream input)
        {
            XmlSerializer _serializer = new XmlSerializer(typeof (XmlReceiveMessageEnvelope));

            object obj = _serializer.Deserialize(input);

            if (obj.GetType() != typeof(XmlReceiveMessageEnvelope))
                throw new SerializationException("An unknown message type was received");

            XmlReceiveMessageEnvelope envelope = (XmlReceiveMessageEnvelope) obj;

            Type t = Type.GetType(envelope.MessageType, true, true);
            if (typeof(T) != t)
                throw new SerializationException("An unexpected message type was received");

            XmlAttributes attributes = new XmlAttributes();
            attributes.XmlRoot= new XmlRootAttribute("Message");

            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            overrides.Add(t, attributes);

            XmlSerializer messageSerializer = new XmlSerializer(t, overrides);

            using(var reader = new XmlNodeReader(envelope.Message))
            {
                obj = messageSerializer.Deserialize(reader);    
            }

            if (obj.GetType() != typeof(T))
                throw new SerializationException("Invalid type");

            return (T) obj;
        }
    }

    [XmlRoot(ElementName = "MessageEnvelope")]
    public class XmlMessageEnvelope
    {
        protected XmlMessageEnvelope()
        {
        }

        public XmlMessageEnvelope(object message)
        {
            Message = message;
            MessageType = message.GetType().FullName;
        }

        public string MessageType { get; set; }

        public object Message { get; set; }
    }

    [XmlRoot(ElementName = "MessageEnvelope")]
    public class XmlReceiveMessageEnvelope
    {
        public string MessageType { get; set; }

        [XmlAnyElement]
        public XmlNode Message { get; set; }
    }


    public interface IMessageSerializer : IDisposable
    {
        void Serialize<T>(Stream output, T message);
        T Deserialize<T>(Stream input);
    }
}