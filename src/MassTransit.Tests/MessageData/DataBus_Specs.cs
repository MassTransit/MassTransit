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
namespace MassTransit.Tests.MessageData
{
    namespace DataBus_Specs
    {
        using System;
        using System.IO;
        using System.Text;
        using System.Threading.Tasks;
        using MassTransit.MessageData;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;


        [TestFixture]
        public class Receiving_a_large_message_with_data :
            InMemoryTestFixture
        {
            [Test]
            public async void Should_load_the_data_from_the_repository()
            {
                string data = NewId.NextGuid().ToString();
                Uri dataAddress;
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new SendMessageWithBigData {Body = new ConstantMessageData<string>(dataAddress, data)};

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<MessageWithBigData> received = await _received;

                string value = await received.Message.Body.Value;
                value.ShouldBe(data);

                Console.WriteLine(value);
            }

            IMessageDataRepository _messageDataRepository;

            Task<ConsumeContext<MessageWithBigData>> _received;

            protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData<MessageWithBigData>(_messageDataRepository);

                _received = Handled<MessageWithBigData>(configurator);
            }
        }

        [TestFixture]
        public class Receiving_a_large_message_with_data_bytes :
            InMemoryTestFixture
        {
            [Test]
            public async void Should_load_the_data_from_the_repository()
            {
                var nextGuid = NewId.NextGuid();
                string data = nextGuid.ToString();
                Uri dataAddress;
                using (var stream = new MemoryStream(nextGuid.ToByteArray(), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new MessageWithByteArrayImpl { Bytes = new ConstantMessageData<byte[]>(dataAddress, nextGuid.ToByteArray()) };

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<MessageWithByteArray> received = await _received;

                byte[] value = await received.Message.Bytes.Value;
                var newId = new NewId(value);
                newId.ToString().ShouldBe(data);

                Console.WriteLine(value);
            }

            IMessageDataRepository _messageDataRepository;

            Task<ConsumeContext<MessageWithByteArray>> _received;

            protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData<MessageWithByteArray>(_messageDataRepository);

                _received = Handled<MessageWithByteArray>(configurator);
            }
        }


        public interface MessageWithByteArray
        {
            MessageData<byte[]> Bytes { get; }
        }


        class MessageWithByteArrayImpl : 
            MessageWithByteArray
        {
            public MessageData<byte[]> Bytes { get; set; }
        }


        public interface MessageWithBigData
        {
            MessageData<string> Body { get; }
        }


        class SendMessageWithBigData :
            MessageWithBigData
        {
            public MessageData<string> Body { get; set; }
        }
    }
}