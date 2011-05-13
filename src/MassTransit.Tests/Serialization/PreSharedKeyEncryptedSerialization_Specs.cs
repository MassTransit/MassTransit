// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Magnum.Extensions;
    using MassTransit.Serialization;
    using Messages;
    using NUnit.Framework;

    public abstract class PreSharedKeyEncryptedSerialization_Specs<TSerializer> where TSerializer : IMessageSerializer, new()
    {
        [SetUp]
        public void SetupContext()
        {
            _message = new PartialSerializationTestMessage
                       {
                           DecimalValue = 123.45m,
                           LongValue = 098123213,
                           BoolValue = true,
                           ByteValue = 127,
                           IntValue = 123,
                           DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
                           GuidValue = new Guid("B00C3BD0-3CE9-4B14-9EC6-E7348084EF1F"),
                           StringValue = "Chris's Sample Code",
                           DoubleValue = 1823.172,
                       };
        }

        private PartialSerializationTestMessage _message;

        [Test]
        public void The_encrypted_serializer_should_be_awesome()
        {
            byte[] serializedMessageData;
            string key = "eguhidbehumjdemy1234567890123456";

            var serializer = new PreSharedKeyEncryptedMessageSerializer(key, new TSerializer());

            using (var output = new MemoryStream())
            {
                serializer.Serialize(output, _message.ToSendContext());

                serializedMessageData = output.ToArray();

      //          Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            var deserializer = new PreSharedKeyEncryptedMessageSerializer(key, new TSerializer());

            using (var input = new MemoryStream(serializedMessageData))
            {
                var receivedMessage = deserializer.Deserialize(input.ToReceiveContext()) as PartialSerializationTestMessage;

                Assert.AreEqual(_message, receivedMessage);
            }
        }
    }

    [TestFixture]
    public class WhenUsingCustomXmlWithEncryption : 
        PreSharedKeyEncryptedSerialization_Specs<XmlMessageSerializer>
    {
    }
    [TestFixture]
    public class WhenUsingDotNotXmlWithEncryption : 
        PreSharedKeyEncryptedSerialization_Specs<DotNotXmlMessageSerializer>
    {
    }
    [TestFixture]
    public class WhenUsingBinaryWithEncryption : 
        PreSharedKeyEncryptedSerialization_Specs<BinaryMessageSerializer>
    {
    }
    [TestFixture]
    public class WhenUsingJsonWithEncryption : 
        PreSharedKeyEncryptedSerialization_Specs<JsonMessageSerializer>
    {
    }
}