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
    using System.Text;
    using MassTransit.ServiceBus.Serialization;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class MessageSerialization_Specs
    {
        [SetUp]
        public void SetupContext()
        {
            _message = new SerializationTestMessage
                {
                    Amount = 123.45m,
                    BigCount = 098123213,
                    Count = 123,
                    Created = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Name = "Chris",
                    Radians = 1823.172,
                };
        }

        private SerializationTestMessage _message;

        [Test]
        public void FIRST_TEST_NAME()
        {
            byte[] serializedMessageData;

            IMessageSerializer serializer = new XmlMessageSerializer();

            using (MemoryStream output = new MemoryStream())
            {
                serializer.Serialize(output, _message);

                serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            using (MemoryStream input = new MemoryStream(serializedMessageData))
            {
                SerializationTestMessage receivedMessage = serializer.Deserialize(input) as SerializationTestMessage;

                Assert.AreEqual(_message, receivedMessage);
            }
        }

        [Test]
        public void The_binary_formatter_should_make_mouths_happy()
        {
            byte[] serializedMessageData;

            IMessageSerializer serializer = new BinaryMessageSerializer();

            using (MemoryStream output = new MemoryStream())
            {
                serializer.Serialize(output, _message);

                serializedMessageData = output.ToArray();
            }

            using (MemoryStream input = new MemoryStream(serializedMessageData))
            {
                SerializationTestMessage receivedMessage = serializer.Deserialize(input) as SerializationTestMessage;

                Assert.AreEqual(_message, receivedMessage);
            }
        }
    }
}