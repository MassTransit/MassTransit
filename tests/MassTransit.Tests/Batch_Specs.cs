namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Receiving_a_single_message_in_a_batch_and_it_faults :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> batch = await _errorHandler;
        }

        public Receiving_a_single_message_in_a_batch_and_it_faults()
        {
            InMemoryTestHarness.TestTimeout = TimeSpan.FromSeconds(5);
        }

        FailingBatchConsumer _consumer;
        Task<ConsumeContext<PingMessage>> _errorHandler;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new FailingBatchConsumer();

            configurator.Batch<PingMessage>(x =>
            {
                x.MessageLimit = 2;
                x.TimeLimit = TimeSpan.FromMilliseconds(500);

                x.Consumer(() => _consumer);
            });
        }
    }


    [TestFixture]
    public class Receiving_a_single_message_in_a_single_message_batch_and_it_faults :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> batch = await _errorHandler;
        }

        FailingBatchConsumer _consumer;
        Task<ConsumeContext<PingMessage>> _errorHandler;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new FailingBatchConsumer();

            configurator.Batch<PingMessage>(x =>
            {
                x.MessageLimit = 1;
                x.TimeLimit = TimeSpan.FromMilliseconds(500);

                x.Consumer(() => _consumer);
            });
        }
    }


    [TestFixture]
    [Category("Flaky")]
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

            var count = await BusTestHarness.Consumed.SelectAsync<PingMessage>().Take(6).Count();

            Assert.Multiple(() =>
            {
                Assert.That(count, Is.EqualTo(6));

                Assert.That(_batches.Select(x => x.Length), Is.EquivalentTo(new[] { 1, 2, 3 }));
            });
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
    [Category("Flaky")]
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

            var count = await BusTestHarness.Consumed.SelectAsync<PingMessage>().Take(6).Count();

            Assert.Multiple(() =>
            {
                Assert.That(count, Is.EqualTo(6));
                Assert.That(_batches.Select(x => x.Length), Is.EquivalentTo(new[] { 1, 2, 3 }));
            });
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
                },
                cc => cc.Options<BatchOptions>(x =>
                    x.SetTimeLimit(TimeSpan.FromMilliseconds(500)).GroupBy<PingMessage, string>(ctx => ctx.CorrelationId?.ToString("D"))));
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

            Assert.That(batch, Has.Length.EqualTo(4));
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

            Assert.That(_duplicateMessages, Is.Empty);
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


    [TestFixture]
    public class Duplicate_messages_by_id_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_single_message_within_same_message_id()
        {
            var correlation1 = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));

            await InactivityTask;

            var count = await BusTestHarness.Consumed.SelectAsync<PingMessage>().Count();

            Assert.That(count, Is.EqualTo(1));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() =>
            {
                TaskCompletionSource<Batch<PingMessage>> tcs = GetTask<Batch<PingMessage>>();
                return new TestBatchConsumer(tcs);
            }, cc => cc.Options<BatchOptions>(x =>
            {
                x.TimeLimit = TimeSpan.FromSeconds(1);
                x.MessageLimit = 2;
            }));
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
