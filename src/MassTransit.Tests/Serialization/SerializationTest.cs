// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Serialization;
    using MassTransit.Transports;
    using MassTransit.Transports.InMemory;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    public abstract class SerializationTest :
        InMemoryTestFixture
    {
        readonly Type _serializerType;
        protected IMessageDeserializer Deserializer;
        protected IMessageSerializer Serializer;
        readonly Uri _sourceAddress = new Uri("loopback://localhost/source");
        readonly Uri _destinationAddress = new Uri("loopback://localhost/destination");
        readonly Uri _responseAddress = new Uri("loopback://localhost/response");
        readonly Uri _faultAddress = new Uri("loopback://localhost/fault");
        readonly Guid _requestId = Guid.NewGuid();

        public SerializationTest(Type serializerType)
        {
            _serializerType = serializerType;
        }

        [TestFixtureSetUp]
        public void SetupSerializationTest()
        {
            if (_serializerType == typeof(JsonMessageSerializer))
            {
                Serializer = new JsonMessageSerializer();
                Deserializer = new JsonMessageDeserializer(JsonMessageSerializer.Deserializer, Bus, Bus);
            }
            else if (_serializerType == typeof(BsonMessageSerializer))
            {
                Serializer = new BsonMessageSerializer();
                Deserializer = new BsonMessageDeserializer(BsonMessageSerializer.Deserializer, Bus, Bus);
            }
            else if (_serializerType == typeof(XmlMessageSerializer))
            {
                Serializer = new XmlMessageSerializer();
                Deserializer = new XmlMessageDeserializer(JsonMessageSerializer.Deserializer, Bus, Bus);
            }
            else if (_serializerType == typeof(EncryptedMessageSerializer))
            {
                ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider();
                var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");

                Serializer = new EncryptedMessageSerializer(streamProvider);
                Deserializer = new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, Bus, Bus, streamProvider);
            }
            else
                throw new ArgumentException("The serializer type is unknown");
        }

        protected T SerializeAndReturn<T>(T obj)
            where T : class
        {
            byte[] serializedMessageData;

            using (var output = new MemoryStream())
            {
                var sendContext = new InMemorySendContext<T>(obj);

                sendContext.SourceAddress = _sourceAddress;
                sendContext.DestinationAddress = _destinationAddress;
                sendContext.FaultAddress = _faultAddress;
                sendContext.ResponseAddress = _responseAddress;
                sendContext.RequestId = _requestId;


                Serializer.Serialize(output, sendContext);

                serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            var message = new InMemoryTransportMessage(Guid.NewGuid(), serializedMessageData, Serializer.ContentType.MediaType);
            var receiveContext = new InMemoryReceiveContext(new Uri("loopback://localhost/input_queue"), message);

            ConsumeContext consumeContext = Deserializer.Deserialize(receiveContext);

            ConsumeContext<T> messageContext;
            consumeContext.TryGetMessage(out messageContext);

            messageContext.ShouldNotBe(null);

            messageContext.SourceAddress.ShouldBe(_sourceAddress);
            messageContext.DestinationAddress.ShouldBe(_destinationAddress);
            messageContext.FaultAddress.ShouldBe(_faultAddress);
            messageContext.ResponseAddress.ShouldBe(_responseAddress);
            messageContext.RequestId.HasValue.ShouldBe(true);
            messageContext.RequestId.Value.ShouldBe(_requestId);

            return messageContext.Message;
        }

        protected void TestSerialization<T>(T message)
            where T : class
        {
            T result = SerializeAndReturn(message);

            message.Equals(result).ShouldBe(true);
        }
    }
}