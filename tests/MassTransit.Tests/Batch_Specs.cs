namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
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


    [TestFixture]
    public class Receiving_a_single_message_in_a_batch_by_convention :
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

        [Test, Explicit]
        public async Task Should_show_me_the_pipe()
        {
            Console.WriteLine(Bus.GetProbeResult().ToJsonString());
        }

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
        TestBatchConsumer _consumer;

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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new TestBatchConsumer(GetTask<Batch<PingMessage>>());

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


    [TestFixture, Explicit]
    public class Using_a_batch_consumer :
        InMemoryTestFixture
    {
        TestBatchConsumer _consumer;

        [Test]
        public async Task Should_show_me_the_pipe()
        {
            Console.WriteLine(Bus.GetProbeResult().ToJsonString());
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
