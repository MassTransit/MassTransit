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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_an_object_to_the_local_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_interface_of_the_message()
        {
            Task<ConsumeContext<IMessageA>> handler = SubscribeHandler<IMessageA>();

            var message = new MessageA();
            await BusSendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async Task Should_receive_the_interface_proxy()
        {
            Task<ConsumeContext<IMessageA>> handler = SubscribeHandler<IMessageA>();

            await BusSendEndpoint.Send<IMessageA>(new {});

            await handler;
        }

        [Test]
        public async Task Should_receive_the_proper_message()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            object message = new MessageA();
            await BusSendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async Task Should_receive_the_proper_message_as_a()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            var message = new MessageA();
            await BusSendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async Task Should_receive_the_proper_message_as_a_with_request_id()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>(x => x.RequestId.HasValue);

            var requestId = NewId.NextGuid();

            var message = new MessageA();
            await BusSendEndpoint.Send(message, c => c.RequestId = requestId);

            var consumeContext = await handler;

            consumeContext.RequestId.ShouldBe(requestId);
        }

        [Test]
        public async Task Should_receive_the_proper_message_type()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            var requestId = NewId.NextGuid();

            object message = new MessageA();
            await BusSendEndpoint.Send(message, typeof(MessageA), c => c.RequestId = requestId);

            var consumeContext = await handler;

            consumeContext.RequestId.ShouldBe(requestId);
        }

        [Test]
        public async Task Should_receive_the_proper_message_without_type()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            var requestId = NewId.NextGuid();

            object message = new MessageA();
            await BusSendEndpoint.Send(message, (SendContext context) => context.RequestId = requestId);

            var consumeContext = await handler;

            consumeContext.RequestId.ShouldBe(requestId);
        }
    }
}