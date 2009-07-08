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
namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Magnum.Cryptography;
    using Magnum.DateTimeExtensions;
    using MassTransit.Serialization;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class PreSharedKeyEncryptedSerialization_Specs
    {
        [SetUp]
        public void SetupContext()
        {
            _message = new SerializationTestMessage
                       {
                           DecimalValue = 123.45m,
                           LongValue = 098123213,
                           BoolValue = true,
                           ByteValue = 127,
                           IntValue = 123,
                           DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
                           TimeSpanValue = 30.Seconds(),
                           GuidValue = new Guid("B00C3BD0-3CE9-4B14-9EC6-E7348084EF1F"),
                           StringValue = "Chris's Sample Code",
                           DoubleValue = 1823.172,
                       };
        }

        private SerializationTestMessage _message;

        [Test, Explicit]
        public void The_encrypted_serializer_should_be_awesome()
        {
            byte[] serializedMessageData;
            string key = "eguhidbehumjdemy1234567890123456";
            var xml = new XmlMessageSerializer();
            var crypto = new RijndaelCryptographyService(key);

            var serializer = new PreSharedKeyEncryptedMessageSerializer(xml, crypto);

            using (MemoryStream output = new MemoryStream())
            {
                serializer.Serialize(output, _message);

                serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            var deserializer = new PreSharedKeyEncryptedMessageSerializer(new XmlMessageSerializer(), new RijndaelCryptographyService(key));
            using (MemoryStream input = new MemoryStream(serializedMessageData))
            {
                SerializationTestMessage receivedMessage = deserializer.Deserialize(input) as SerializationTestMessage;

                Assert.AreEqual(_message, receivedMessage);
            }
        }
    }
}