// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    namespace Batching
    {
        using System;
        using System.Linq;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using TestFramework.Messages;
        using Util;


        [TestFixture]
        public class When_a_batch_limit_is_reached :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_receive_the_message_batch()
            {
                await Task.WhenAll(Enumerable.Range(0, 5).Select(x => InputQueueSendEndpoint.Send(new PingMessage())));

                Batch<PingMessage> batch = await _consumer[0];

                Assert.That(batch.Length, Is.EqualTo(5));
                Assert.That(batch.Mode, Is.EqualTo(BatchCompletionMode.Size));
            }

            TestBatchConsumer _consumer;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

                configurator.PrefetchCount = 10;

                configurator.Batch<PingMessage>(x =>
                {
                    x.MessageLimit = 5;

                    x.Consumer(() => _consumer);
                });
            }
        }


        [TestFixture]
        public class When_a_batch_timeout_is_reached_due_to_prefetch :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_receive_the_message_batch()
            {
                for (int i = 0; i < 10; i++)
                    await InputQueueSendEndpoint.Send(new PingMessage());

                Batch<PingMessage> batch = await _consumer[0];

                Assert.That(batch.Length, Is.EqualTo(5));
                Assert.That(batch.Mode, Is.EqualTo(BatchCompletionMode.Time));

                batch = await _consumer[1];

                Assert.That(batch.Length, Is.EqualTo(5));
                Assert.That(batch.Mode, Is.EqualTo(BatchCompletionMode.Time));
            }

            TestBatchConsumer _consumer;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>(), GetTask<Batch<PingMessage>>());

                configurator.PrefetchCount = 5;

                configurator.Batch<PingMessage>(x =>
                {
                    x.MessageLimit = 10;
                    x.TimeLimit = TimeSpan.FromSeconds(1);

                    x.Consumer(() => _consumer);
                });
            }
        }


        class TestBatchConsumer :
            IConsumer<Batch<PingMessage>>
        {
            readonly TaskCompletionSource<Batch<PingMessage>>[] _messageTask;

            int _count;

            public TestBatchConsumer(params TaskCompletionSource<Batch<PingMessage>>[] messageTask)
            {
                _messageTask = messageTask;
            }

            public Task<Batch<PingMessage>> this[int index] => _messageTask[index].Task;

            public Task Consume(ConsumeContext<Batch<PingMessage>> context)
            {
                if (_count < _messageTask.Length)
                    _messageTask[_count++].TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }
}
