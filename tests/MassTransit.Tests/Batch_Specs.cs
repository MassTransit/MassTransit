namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Microsoft.VisualStudio.TestPlatform.Utilities;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class When_a_batch_limit_is_reached :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(2));
        }

        TestBatchConsumer _consumer;

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
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => InputQueueSendEndpoint.Send(new PingMessage())));

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(100));
        }

        TestBatchConsumer _consumer;

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
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(1));
        }

        TestBatchConsumer _consumer;

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


    [TestFixture]
    public class Receiving_a_single_message_in_a_batch_by_convention :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(1));
        }

        [Test]
        [Explicit]
        public async Task Should_show_me_the_pipe()
        {
            Console.WriteLine(Bus.GetProbeResult().ToJsonString());
        }

        TestBatchConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.UseMessageRetry(r => r.Immediate(2));

            configurator.Consumer(() => _consumer, cc => cc.Options<BatchOptions>(x => x.SetTimeLimit(500).SetMessageLimit(2)));
        }
    }


    [TestFixture]
    public class Receiving_a_bunch_of_messages_in_a_batch_by_convention :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(4));
        }

        TestBatchConsumer _consumer;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.TransportConcurrencyLimit = 16;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.Consumer(() => _consumer);
        }
    }


    [TestFixture]
    public class Configuring_the_batch_consumer_pipeline :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_the_outbox()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(4));
        }

        TestOutboxBatchConsumer _consumer;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.TransportConcurrencyLimit = 16;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestOutboxBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.Consumer(() => _consumer, x =>
            {
                x.Message<Batch<PingMessage>>(m => m.UseInMemoryOutbox());
            });
        }
    }


    [TestFixture]
    public class Configuring_the_batch_endpoint_pipeline :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_the_outbox()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(4));
        }

        TestOutboxBatchConsumer _consumer;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.TransportConcurrencyLimit = 16;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestOutboxBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.UseInMemoryOutbox();

            configurator.Consumer(() => _consumer);
        }
    }


    [TestFixture]
    public class Configuring_the_batch_endpoint_pipeline_with_retry :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_the_outbox_and_retry_the_same_batch()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(4));
        }

        TestRetryOutboxBatchConsumer _consumer;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.TransportConcurrencyLimit = 16;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestRetryOutboxBatchConsumer(GetTask<Batch<PingMessage>>());

            configurator.UseMessageRetry(r => r.Immediate(2));
            configurator.UseInMemoryOutbox();

            configurator.Consumer(() => _consumer);
        }
    }


    [TestFixture]
    public class Receiving_a_bunch_of_messages_in_a_batch_by_convention_using_mediator :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            var consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Consumer(() => consumer);
            });

            await Task.WhenAll(mediator.Send(new PingMessage()),
                mediator.Send(new PingMessage()),
                mediator.Send(new PingMessage()),
                mediator.Send(new PingMessage()));

            Batch<PingMessage> batch = await consumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(4));
        }
    }


    [TestFixture]
    [Explicit]
    public class Using_a_batch_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_deliver_duplicate_messages()
        {
            IEnumerable<DoWork> messages = Enumerable.Range(0, Count).Select(x => new DoWork());
            foreach (var msg in messages)
                await InputQueueSendEndpoint.Send(msg);

            await _completed.Task;

            Assert.That(_duplicateMessages.Count, Is.EqualTo(0));
        }

        public Using_a_batch_consumer()
        {
            TestTimeout = TimeSpan.FromMinutes(2);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        readonly HashSet<Guid> _alreadyReceivedMessages = new HashSet<Guid>();
        readonly HashSet<Guid> _duplicateMessages = new HashSet<Guid>();
        TaskCompletionSource<int> _completed;
        const int Count = 15000;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<int>();

            configurator.Batch<DoWork>(x =>
            {
                x.MessageLimit = 100;
                x.TimeLimit = TimeSpan.FromMilliseconds(50);

                x.Consumer(() => new DoWorkConsumer(_alreadyReceivedMessages, _duplicateMessages, _completed));
            });
        }


        public class DoWork
        {
        }


        class DoWorkConsumer :
            IConsumer<Batch<DoWork>>
        {
            readonly HashSet<Guid> _alreadyReceivedMessages;
            readonly TaskCompletionSource<int> _completed;
            readonly HashSet<Guid> _duplicateMessages;

            public DoWorkConsumer(HashSet<Guid> alreadyReceivedMessages, HashSet<Guid> duplicateMessages, TaskCompletionSource<int> completed)
            {
                _alreadyReceivedMessages = alreadyReceivedMessages;
                _duplicateMessages = duplicateMessages;
                _completed = completed;
            }

            public async Task Consume(ConsumeContext<Batch<DoWork>> context)
            {
                lock (_alreadyReceivedMessages)
                {
                    foreach (ConsumeContext<DoWork> msg in context.Message)
                    {
                        if (_alreadyReceivedMessages.Contains(msg.MessageId.Value))
                        {
                            _duplicateMessages.Add(msg.MessageId.Value);

                            return;
                        }

                        _alreadyReceivedMessages.Add(msg.MessageId.Value);
                        if (_alreadyReceivedMessages.Count == Count)
                            _completed.TrySetResult(Count);
                    }
                }

                for (var i = 0; i < 50000000; i++)
                {
                    if (i % 5000000 == 0)
                        await Task.Yield();
                }
            }
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

        public Task<Batch<PingMessage>> Completed => _messageTask.Task;

        public Task Consume(ConsumeContext<Batch<PingMessage>> context)
        {
            _messageTask.TrySetResult(context.Message);

            return TaskUtil.Completed;
        }
    }


    class TestOutboxBatchConsumer :
        IConsumer<Batch<PingMessage>>
    {
        readonly TaskCompletionSource<Batch<PingMessage>> _messageTask;

        public TestOutboxBatchConsumer(TaskCompletionSource<Batch<PingMessage>> messageTask)
        {
            _messageTask = messageTask;
        }

        public Task<Batch<PingMessage>> Completed => _messageTask.Task;

        public Task Consume(ConsumeContext<Batch<PingMessage>> context)
        {
            if (context.TryGetPayload<InMemoryOutboxConsumeContext>(out var outboxContext))
                _messageTask.TrySetResult(context.Message);
            else
                _messageTask.TrySetException(new InvalidOperationException("Outbox context is not available at this point"));

            return TaskUtil.Completed;
        }
    }


    class TestRetryOutboxBatchConsumer :
        IConsumer<Batch<PingMessage>>
    {
        readonly TaskCompletionSource<Batch<PingMessage>> _messageTask;
        int _attempt;

        public TestRetryOutboxBatchConsumer(TaskCompletionSource<Batch<PingMessage>> messageTask)
        {
            _messageTask = messageTask;
        }

        public Task<Batch<PingMessage>> Completed => _messageTask.Task;

        public Task Consume(ConsumeContext<Batch<PingMessage>> context)
        {
            if (context.TryGetPayload<InMemoryOutboxConsumeContext>(out var outboxContext))
            {
                if (Interlocked.Increment(ref _attempt) == 1)
                {
                    throw new Exception("Force Retry");
                }

                _messageTask.TrySetResult(context.Message);
            }
            else
                _messageTask.TrySetException(new InvalidOperationException("Outbox context is not available at this point"));

            return TaskUtil.Completed;
        }
    }
}
