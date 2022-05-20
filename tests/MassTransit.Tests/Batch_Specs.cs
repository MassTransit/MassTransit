namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Middleware.InMemoryOutbox;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


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
    public class When_a_batch_consumer_is_being_tested
    {
        [Test]
        [Order(2)]
        public async Task Should_have_called_the_consumer_method()
        {
            Assert.That(await _consumer.Consumed.SelectAsync<Batch<PingMessage>>().Count(), Is.EqualTo(1));
        }

        [Test]
        [Order(1)]
        public async Task Should_have_the_goods()
        {
            await _harness.InputQueueSendEndpoint.Send(new PingMessage());
            await _harness.InputQueueSendEndpoint.Send(new PingMessage());

            Batch<PingMessage> batch = await _batchConsumer.Completed;

            Assert.That(batch.Length, Is.EqualTo(2));
        }

        [Test]
        [Order(5)]
        public async Task Should_send_the_initial_message_to_the_consumer()
        {
            Assert.That(await _harness.Sent.SelectAsync<PingMessage>().Count(), Is.EqualTo(2));
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestBatchConsumer> _consumer;
        TestBatchConsumer _batchConsumer;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _batchConsumer = new TestBatchConsumer(_harness.GetTask<Batch<PingMessage>>());
            _consumer = _harness.Consumer(() => _batchConsumer, c => c.Options<BatchOptions>(o => o.SetMessageLimit(2)));

            await _harness.Start();
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
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
            configurator.ConcurrentMessageLimit = 200;
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
            configurator.ConcurrentMessageLimit = 16;
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
            configurator.ConcurrentMessageLimit = 16;
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
            configurator.ConcurrentMessageLimit = 16;
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
            configurator.ConcurrentMessageLimit = 16;
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
    public class Processing_a_failing_batch_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_comply_with_retry_policy()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _task.Task;

            Assert.That(_consumer.Attempts, Is.EqualTo(2));
        }

        FailingBatchConsumer _consumer;
        TaskCompletionSource<ConsumeContext<Fault<PingMessage>>> _task;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new FailingBatchConsumer();

            configurator.UseMessageRetry(r => r.Immediate(1));

            configurator.Consumer(() => _consumer);

            _task = GetTask<ConsumeContext<Fault<PingMessage>>>();
            configurator.Handler<Fault<PingMessage>>(async m => _task.SetResult(m));
        }
    }


    [TestFixture]
    public class Processing_a_failing_batch_with_retry_and_delayed_redelivery :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_comply_with_retry_policy()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(_firstId));
            await InputQueueSendEndpoint.Send(new PingMessage(_secondId));

            await _firstFault.Task;
            await _secondFault.Task;

            Assert.That(_consumer.Attempts, Is.EqualTo(4));
        }

        public Processing_a_failing_batch_with_retry_and_delayed_redelivery()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(8);
            _firstFault = GetTask<ConsumeContext<Fault<PingMessage>>>();
            _secondFault = GetTask<ConsumeContext<Fault<PingMessage>>>();
            _firstId = NewId.NextGuid();
            _secondId = NewId.NextGuid();
        }

        FailingBatchConsumer _consumer;

        readonly TaskCompletionSource<ConsumeContext<Fault<PingMessage>>> _firstFault;
        readonly TaskCompletionSource<ConsumeContext<Fault<PingMessage>>> _secondFault;
        readonly Guid _firstId;
        readonly Guid _secondId;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new FailingBatchConsumer();

            configurator.UseDelayedRedelivery(r => r.Intervals(100));
            configurator.UseMessageRetry(r => r.Immediate(1));

            configurator.Consumer(() => _consumer);

            configurator.Handler<Fault<PingMessage>>(async m =>
            {
                if (_firstId == m.Message.Message.CorrelationId)
                    _firstFault.SetResult(m);
                if (_secondId == m.Message.Message.CorrelationId)
                    _secondFault.SetResult(m);
            });
        }
    }


    [TestFixture]
    public class Processing_a_failing_batch_with_retry_and_scheduled_redelivery :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_comply_with_retry_policy()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(_firstId));
            await InputQueueSendEndpoint.Send(new PingMessage(_secondId));

            await _firstFault.Task;
            await _secondFault.Task;

            Assert.That(_consumer.Attempts, Is.EqualTo(4));
        }

        public Processing_a_failing_batch_with_retry_and_scheduled_redelivery()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(8);
            _firstFault = GetTask<ConsumeContext<Fault<PingMessage>>>();
            _secondFault = GetTask<ConsumeContext<Fault<PingMessage>>>();
            _firstId = NewId.NextGuid();
            _secondId = NewId.NextGuid();
        }

        FailingBatchConsumer _consumer;

        readonly TaskCompletionSource<ConsumeContext<Fault<PingMessage>>> _firstFault;
        readonly TaskCompletionSource<ConsumeContext<Fault<PingMessage>>> _secondFault;
        readonly Guid _firstId;
        readonly Guid _secondId;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new FailingBatchConsumer();

            configurator.UseScheduledRedelivery(r => r.Intervals(100));
            configurator.UseMessageRetry(r => r.Immediate(1));

            configurator.Consumer(() => _consumer);

            configurator.Handler<Fault<PingMessage>>(async m =>
            {
                if (_firstId == m.Message.Message.CorrelationId)
                    _firstFault.SetResult(m);
                if (_secondId == m.Message.Message.CorrelationId)
                    _secondFault.SetResult(m);
            });
        }
    }


    [TestFixture]
    public class Processing_another_failing_batch_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_fault_once_per_message()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            await InactivityTask;

            Assert.That(_individualFaults, Is.EqualTo(2));
            Assert.That(_batchFaults, Is.EqualTo(0));
        }

        int _individualFaults;
        int _batchFaults;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseMessageRetry(r => r.Immediate(1));
            configurator.Consumer(() => new FailingBatchConsumer());

            configurator.Handler<Fault<PingMessage>>(async m => Interlocked.Increment(ref _individualFaults));
            configurator.Handler<Fault<Batch<PingMessage>>>(async m => Interlocked.Increment(ref _batchFaults));
        }
    }


    [TestFixture]
    public class Receiving_and_grouping_messages :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_one_batch_per_group()
        {
            var correlation1 = NewId.NextGuid();
            var correlation2 = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation2));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation2));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation2));

            await InactivityTask;

            var count = await BusTestHarness.Consumed.SelectAsync<PingMessage>().Take(6).Count();

            Assert.That(count, Is.EqualTo(6));

            Assert.That(_batches.Select(x => x.Length), Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        readonly List<Batch<PingMessage>> _batches = new List<Batch<PingMessage>>();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 10;

            configurator.Consumer(() =>
            {
                TaskCompletionSource<Batch<PingMessage>> tcs = GetTask<Batch<PingMessage>>();
                tcs.Task.ContinueWith(t => _batches.Add(t.Result));
                var consumer = new TestBatchConsumer(tcs);
                return consumer;
            }, cc => cc.Options<BatchOptions>(x => x.SetTimeLimit(TimeSpan.FromMilliseconds(300)).GroupBy<PingMessage, Guid>(ctx => ctx.CorrelationId)));
        }
    }


    [TestFixture]
    public class Receiving_and_grouping_messages_by_ref_type :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_one_batch_per_group()
        {
            var correlation1 = NewId.NextGuid();
            var correlation2 = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation2));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation2));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.CorrelationId = correlation2));

            await InactivityTask;

            var count = await BusTestHarness.Consumed.SelectAsync<PingMessage>().Take(6).Count();

            Assert.That(count, Is.EqualTo(6));
            Assert.That(_batches.Select(x => x.Length), Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        readonly List<Batch<PingMessage>> _batches = new List<Batch<PingMessage>>();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 10;

            configurator.Consumer(() =>
            {
                TaskCompletionSource<Batch<PingMessage>> tcs = GetTask<Batch<PingMessage>>();
                tcs.Task.ContinueWith(t => _batches.Add(t.Result));
                var consumer = new TestBatchConsumer(tcs);
                return consumer;
            }, cc => cc.Options<BatchOptions>(x => x.GroupBy<PingMessage, string>(ctx => ctx.CorrelationId?.ToString("D"))));
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

            return Task.CompletedTask;
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

            return Task.CompletedTask;
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
                    throw new Exception("Force Retry");

                _messageTask.TrySetResult(context.Message);
            }
            else
                _messageTask.TrySetException(new InvalidOperationException("Outbox context is not available at this point"));

            return Task.CompletedTask;
        }
    }


    class FailingBatchConsumer :
        IConsumer<Batch<PingMessage>>
    {
        int _attempts;

        public int Attempts => _attempts;

        public Task Consume(ConsumeContext<Batch<PingMessage>> context)
        {
            Interlocked.Increment(ref _attempts);

            throw new Exception("some error");
        }
    }
}
