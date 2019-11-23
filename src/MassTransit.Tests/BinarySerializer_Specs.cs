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
    using TestFramework;
    using Util;
    using MassTransit.Transports.InMemory;
    using System;
    using System.Security.Cryptography.X509Certificates;


    [TestFixture]
    public class When_using_the_binary_serializer :
        InMemoryTestFixture
    {
        readonly TaskCompletionSource<Fault<A>> _faultTaskTcs = TaskUtil.GetTask<Fault<A>>();

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseBinarySerializer();
            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

            configurator.Handler<A>(async m =>
            {
                throw new System.Exception("Booom!");
            });

            configurator.Handler<Fault<A>>(async m =>
            {
                _faultTaskTcs.TrySetResult(m.Message);
            });

        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }


        [System.Serializable]
        class A
        {
        }


        [Test]
        [Category("Unit")]
        public async Task Should_be_possibile_to_send_and_consume_faults()
        {
            await Bus.Publish(new A());
            var faultTask = _faultTaskTcs.Task;
            var completedTask = await Task.WhenAny(faultTask, Task.Delay(2000));
            Assert.AreEqual(faultTask, completedTask);
        }
    }


    [TestFixture]
    public class When_sending_messages_using_the_binary_serializer_between_multiple_bus_istances
    {
        [Serializable]
        public class ListNode
        {
            public ListNode Next { get; set; }
            public int Value { get; set; }
        }


        [Serializable]
        public class Base
        {
            public ListNode Head { get; set; }
            public int PropBase { get; set; }
        }


        [Serializable]
        public class Derived : Base
        {
            public int PropDerived { get; set; }
        }


        [Test]
        public async Task Should_be_able_to_consume_messages_polymorphically_if_the_receiving_bus_support_the_binary_serializer()
        {
            var consumed = TaskUtil.GetTask<Base>();

            var bus = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.SupportBinaryMessageDeserializer();
                x.UseBinarySerializer();
                x.ReceiveEndpoint("input_queue", configurator =>
                {
                #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                    configurator.Handler<Base>(async ctx =>
                    {
                        consumed.TrySetResult(ctx.Message);
                    });
                #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                });
            });

            await bus.StartAsync();
            try
            {
                // Create a recursive list
                var head = new ListNode {Value = 100};
                var tail = new ListNode {Next = head, Value = 200};
                head.Next = tail;

                var messageToSend = new Derived()
                {
                    PropBase = 10,
                    PropDerived = 20,
                    Head = head
                };

                await bus.Publish(messageToSend);

                var completedTask = await Task.WhenAny(consumed.Task, Task.Delay(250));

                Assert.AreEqual(consumed.Task, completedTask,
                    "Timeout while waiting to receive the message sent on the source bus.");

                var message = await consumed.Task;
                Assert.NotNull(message);
                Assert.AreEqual(messageToSend.PropBase, message.PropBase);
                Assert.AreEqual(head.Value, message.Head.Value);
                Assert.AreEqual(tail.Value, message.Head.Next.Value);
                Assert.AreEqual(head.Value, message.Head.Next.Next.Value);
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
