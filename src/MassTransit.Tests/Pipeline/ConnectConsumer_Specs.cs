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
namespace MassTransit.Tests.Pipeline
{
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Sinks;
    using NUnit.Framework;


    [TestFixture]
    public class Connecting_a_consumer_to_the_inbound_pipe :
        AsyncTestFixture
    {
        [Test]
        public async void Should_receive_a_message()
        {
            IInboundMessagePipe pipe = new InboundMessagePipe();

            OneMessageConsumer consumer = GetOneMessageConsumer();

            IAsyncConsumerFactory<OneMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            ConnectHandle connectHandle = pipe.ConnectConsumer(factory);

            var consumeContext = new TestConsumeContext<MessageA>(new MessageA());

            await pipe.Send(consumeContext);

            await consumer.Task;
        }

        [Test]
        public async void Should_receive_a_two_messages()
        {
            IInboundMessagePipe pipe = new InboundMessagePipe();

            TwoMessageConsumer consumer = GetTwoMessageConsumer();

            IAsyncConsumerFactory<TwoMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            ConnectHandle connectHandle = pipe.ConnectConsumer(factory);

            await pipe.Send(new TestConsumeContext<MessageA>(new MessageA()));

            await pipe.Send(new TestConsumeContext<MessageB>(new MessageB()));

            await consumer.TaskA;
            await consumer.TaskB;
        }
    }
}