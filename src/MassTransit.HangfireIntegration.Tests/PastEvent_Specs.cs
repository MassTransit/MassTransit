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
namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using NUnit.Framework;


    [TestFixture]
    public class Specifying_an_event_in_the_past :
        HangfireInMemoryTestFixture
    {
        class A
        {
            public string Name { get; set; }
        }


        [Test, Explicit]
        public async Task Should_be_able_to_cancel_a_future_event()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            var scheduledMessage = await HangfireEndpoint.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(120), new A {Name = "Joe"});

            await Task.Delay(2000);

            await HangfireEndpoint.CancelScheduledSend(scheduledMessage);

            await Task.Delay(2000);
        }

        [Test]
        public async Task Should_handle_now_properly()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            await HangfireEndpoint.ScheduleSend(Bus.Address, DateTime.UtcNow, new A {Name = "Joe"});

            await handler;
        }

        [Test]
        public async Task Should_handle_slightly_in_the_future_properly()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            await HangfireEndpoint.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(2), new A {Name = "Joe"});

            await handler;
        }

        [Test]
        public async Task Should_include_message_headers()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            Guid requestId = Guid.NewGuid();
            Guid correlationId = Guid.NewGuid();
            Guid conversationId = Guid.NewGuid();
            Guid initiatorId = Guid.NewGuid();
            await HangfireEndpoint.ScheduleSend(Bus.Address, DateTime.UtcNow, new A {Name = "Joe"}, Pipe.Execute<SendContext>(x =>
            {
                x.FaultAddress = Bus.Address;
                x.ResponseAddress = InputQueueAddress;
                x.RequestId = requestId;
                x.CorrelationId = correlationId;
                x.ConversationId = conversationId;
                x.InitiatorId = initiatorId;

                x.Headers.Set("Hello", "World");
            }));

            ConsumeContext<A> context = await handler;

            Assert.AreEqual(Bus.Address, context.FaultAddress);
            Assert.AreEqual(InputQueueAddress, context.ResponseAddress);
            Assert.IsTrue(context.RequestId.HasValue);
            Assert.AreEqual(requestId, context.RequestId.Value);
            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.AreEqual(correlationId, context.CorrelationId.Value);
            Assert.IsTrue(context.ConversationId.HasValue);
            Assert.AreEqual(conversationId, context.ConversationId.Value);
            Assert.IsTrue(context.InitiatorId.HasValue);
            Assert.AreEqual(initiatorId, context.InitiatorId.Value);

            object value;
            Assert.IsTrue(context.Headers.TryGetHeader("Hello", out value));
            Assert.AreEqual("World", value);
        }

        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            await HangfireEndpoint.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromHours(-1), new A {Name = "Joe"});

            await handler;
        }
    }


    [TestFixture]
    public class Specifying_an_event_reschedule_if_exists :
        HangfireInMemoryTestFixture
    {
        public Specifying_an_event_reschedule_if_exists()
        {
            ScheduleTokenId.UseTokenId<A>(x => x.Id);
        }


        class A
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }


        [Test]
        public async Task Should_reschedule()
        {
            var handler = SubscribeHandler<A>();
            var id = NewId.NextGuid();
            var expected = "Joe 2";
            await HangfireEndpoint.ScheduleSend(Bus.Address, TimeSpan.FromSeconds(120), new A
            {
                Id = id,
                Name = "Joe"
            });

            await Task.Delay(2000);

            await HangfireEndpoint.ScheduleSend(Bus.Address, TimeSpan.FromSeconds(5), new A
            {
                Id = id,
                Name = expected
            });

            ConsumeContext<A> result = await handler;
            Assert.AreEqual(expected, result.Message.Name);
        }
    }
}
