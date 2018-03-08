// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class When_a_batch_limit_is_reached :
        InMemoryTestFixture
    {
        TestBatchConsumer _consumer;

        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(2));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.Batch<PingMessage>(x =>
            {
                x.MessageLimit = 2;

                x.Consumer(() => _consumer);
            });
        }
    }


    [TestFixture]
    public class When_I_like_big_batches_and_I_cannot_lie :
        InMemoryTestFixture
    {
        TestBatchConsumer _consumer;

        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => InputQueueSendEndpoint.Send(new PingMessage())));

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(100));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.TransportConcurrencyLimit = 200;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.Batch<PingMessage>(x =>
            {
                x.MessageLimit = 100;

                x.Consumer(() => _consumer);
            });
        }
    }


    [TestFixture]
    public class Receiving_a_single_message_in_a_batch :
        InMemoryTestFixture
    {
        TestBatchConsumer _consumer;

        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(1));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.Batch<PingMessage>(x =>
            {
                x.MessageLimit = 2;
                x.TimeLimit = TimeSpan.FromMilliseconds(500);

                x.Consumer(() => _consumer);
            });
        }
    }


    class TestBatchConsumer :
        IConsumer<Batch<PingMessage>>
    {
        readonly TaskCompletionSource<Batch<PingMessage>> _messageTask;

        public TestBatchConsumer(TaskCompletionSource<Batch<PingMessage>> messageTask)
        {
            _messageTask = messageTask;
        }

        public Task Consume(ConsumeContext<Batch<PingMessage>> context)
        {
            _messageTask.TrySetResult(context.Message);

            return TaskUtil.Completed;
        }

        public Task<Batch<PingMessage>> Completed => _messageTask.Task;
    }
}