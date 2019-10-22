// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Pipeline;
    using MassTransit.Serialization;
    using MassTransit.Transports;
    using MassTransit.Transports.InMemory;
    using MassTransit.Transports.InMemory.Contexts;
    using MassTransit.Transports.InMemory.Fabric;
    using Messages;
    using Metadata;
    using NUnit.Framework;
    using Util;


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    [Explicit]
    public class Serializer_performance :
        SerializationTest
    {
        public Serializer_performance(Type serializerType)
            : base(serializerType)
        {
        }

        protected byte[] Serialize<T>(SendContext<T> sendContext)
            where T : class
        {
            using (var output = new MemoryStream())
            {
                Serializer.Serialize(output, sendContext);

                return output.ToArray();
            }
        }

        protected ConsumeContext<T> Deserialize<T>(ReceiveContext receiveContext)
            where T : class
        {
            ConsumeContext consumeContext = Deserializer.Deserialize(receiveContext);

            ConsumeContext<T> messageContext;
            consumeContext.TryGetMessage(out messageContext);

            return messageContext;
        }

        [Test]
        public void Just_how_fast_are_you()
        {
            var message = new SerializationTestMessage
            {
                DecimalValue = 123.45m,
                LongValue = 098123213,
                BoolValue = true,
                ByteValue = 127,
                IntValue = 123,
                DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
                TimeSpanValue = TimeSpan.FromSeconds(30),
                GuidValue = Guid.NewGuid(),
                StringValue = "Chris's Sample Code",
                DoubleValue = 1823.172,
            };

            var sendContext = new InMemorySendContext<SerializationTestMessage>(message);
            ReceiveContext receiveContext = null;
            //warm it up
            for (int i = 0; i < 10; i++)
            {
                byte[] data = Serialize(sendContext);

                var transportMessage = new InMemoryTransportMessage(Guid.NewGuid(), data, Serializer.ContentType.MediaType, TypeMetadataCache<SerializationTestMessage>.ShortName);
                receiveContext = new InMemoryReceiveContext(new Uri("loopback://localhost/input_queue"), transportMessage, null);

                Deserialize<SerializationTestMessage>(receiveContext);
            }

            Stopwatch timer = Stopwatch.StartNew();

            const int iterations = 50000;

            for (int i = 0; i < iterations; i++)
            {
                Serialize(sendContext);
            }

            timer.Stop();

            long perSecond = iterations * 1000 / timer.ElapsedMilliseconds;

            Console.WriteLine("Serialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);


            timer = Stopwatch.StartNew();

            for (int i = 0; i < 50000; i++)
            {
                Deserialize<SerializationTestMessage>(receiveContext);
            }

            timer.Stop();

            perSecond = iterations * 1000 / timer.ElapsedMilliseconds;

            Console.WriteLine("Deserialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
        }
    }
}