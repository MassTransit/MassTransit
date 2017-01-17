// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ProtocolBuffers.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Pipeline;
    using Pipeline.Observables;
    using Shouldly;
    using TestFramework;
    using Transports.InMemory;
    using Util;


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
        protected readonly Guid _requestId = Guid.NewGuid();

        public SerializationTest(Type serializerType)
        {
            _serializerType = serializerType;
        }

        [OneTimeSetUp]
        public void SetupSerializationTest()
        {
            if (_serializerType == typeof(ProtocolBuffersMessageSerializer))
            {
                Serializer = new ProtocolBuffersMessageSerializer();
                Deserializer = new ProtocolBuffersMessageDeserializer();
            }
            else
                throw new ArgumentException("The serializer type is unknown");
        }

        protected T SerializeAndReturn<T>(T obj)
            where T : class
        {

            var serializedMessageData = Serialize(obj);

            return Return<T>(serializedMessageData);
        }

        protected byte[] Serialize<T>(T obj)
            where T : class
        {
            using (var output = new MemoryStream())
            {
                var sendContext = new InMemorySendContext<T>(obj);

                sendContext.SourceAddress = _sourceAddress;
                sendContext.DestinationAddress = _destinationAddress;
                sendContext.FaultAddress = _faultAddress;
                sendContext.ResponseAddress = _responseAddress;
                sendContext.RequestId = _requestId;


                Serializer.Serialize(output, sendContext);

                byte[] serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
                return serializedMessageData;
            }
        }

        protected T Return<T>(byte[] serializedMessageData)
            where T : class
        {
            var message = new InMemoryTransportMessage(Guid.NewGuid(), serializedMessageData, Serializer.ContentType.MediaType, TypeMetadataCache<T>.ShortName);
            var receiveContext = new InMemoryReceiveContext(new Uri("loopback://localhost/input_queue"), message, new ReceiveObservable(), null);

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

        protected virtual void TestSerialization<T>(T message)
            where T : class
        {
            T result = SerializeAndReturn(message);

            message.Equals(result).ShouldBe(true);
        }
    }
}