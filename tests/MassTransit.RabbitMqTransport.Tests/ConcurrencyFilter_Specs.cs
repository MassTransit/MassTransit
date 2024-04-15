namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    [Category("Flaky")]
    public class Using_a_consumer_concurrency_limit :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_limit_the_consumer()
        {
            _complete = GetTask<bool>();

            var tasks = new List<Task>(_messageCount * 2);

            for (var i = 0; i < _messageCount; i++)
            {
                tasks.Add(Bus.Publish(new A()));
                tasks.Add(Bus.Publish(new B()));
            }

            await Task.WhenAll(tasks);

            await _complete.Task;

            Assert.That(_consumer.MaxDeliveryCount, Is.EqualTo(2));
        }

        Consumer _consumer;
        static readonly int _messageCount = 50;
        static TaskCompletionSource<bool> _complete;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _consumer = new Consumer();

            configurator.Instance(_consumer, x => x.UseConcurrentMessageLimit(2));
        }


        class Consumer :
            IConsumer<A>,
            IConsumer<B>
        {
            int _currentPendingDeliveryCount;
            long _deliveryCount;
            int _maxPendingDeliveryCount;

            public int MaxDeliveryCount => _maxPendingDeliveryCount;

            public Task Consume(ConsumeContext<A> context)
            {
                return OnConsume();
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return OnConsume();
            }

            async Task OnConsume()
            {
                Interlocked.Increment(ref _deliveryCount);

                var current = Interlocked.Increment(ref _currentPendingDeliveryCount);
                while (current > _maxPendingDeliveryCount)
                    Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

                await Task.Delay(10);

                Interlocked.Decrement(ref _currentPendingDeliveryCount);

                if (_deliveryCount >= _messageCount * 2)
                    _complete.TrySetResult(true);
            }
        }


        class A
        {
        }


        class B
        {
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class Using_a_consumer_concurrency_limit_set_to_1 :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_limit_the_consumer_and_consume_messages_sequentially()
        {
            _complete = GetTask<bool>();

            int sequenceIndex = 1;
            for (var i = 0; i < _messageCount; i++)
            {
                await Bus.Publish(new A(sequenceIndex++));
                await Bus.Publish(new B(sequenceIndex++));
            }

            await _complete.Task;

            Assert.Multiple(() =>
            {
                Assert.That(_consumer.MaxDeliveryCount, Is.EqualTo(1));
                Assert.That(_consumer.CompletedConsumingSequentially, Is.EqualTo(true));
            });
        }

        Consumer _consumer;
        static readonly int _messageCount = 50;
        static TaskCompletionSource<bool> _complete;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _consumer = new Consumer();
            configurator.ConcurrentMessageLimit = 1;
            configurator.Instance(_consumer, x => x.UseConcurrentMessageLimit(1));
        }


        class Consumer :
            IConsumer<A>,
            IConsumer<B>
        {
            bool _completedConsumingSequentially = true;
            int _currentPendingDeliveryCount;
            long _deliveryCount;
            int _maxPendingDeliveryCount;
            int _previousSequenceIndex;

            public int MaxDeliveryCount => _maxPendingDeliveryCount;
            public bool CompletedConsumingSequentially => _completedConsumingSequentially;

            public Task Consume(ConsumeContext<A> context)
            {
                return OnConsume(context.Message.SequenceIndex);
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return OnConsume(context.Message.SequenceIndex);
            }

            async Task OnConsume(int sequenceIndex)
            {
                Interlocked.Increment(ref _deliveryCount);

                var current = Interlocked.Increment(ref _currentPendingDeliveryCount);
                while (current > _maxPendingDeliveryCount)
                    Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

                await Task.Delay(10);

                Interlocked.Decrement(ref _currentPendingDeliveryCount);

                _completedConsumingSequentially = _completedConsumingSequentially && _previousSequenceIndex == sequenceIndex - 1;
                _previousSequenceIndex = sequenceIndex;

                if (_deliveryCount >= _messageCount * 2)
                    _complete.TrySetResult(true);
            }
        }


        class A
        {
            public A(int sequenceIndex)
            {
                SequenceIndex = sequenceIndex;
            }

            public int SequenceIndex { get; }
        }


        class B
        {
            public B(int sequenceIndex)
            {
                SequenceIndex = sequenceIndex;
            }

            public int SequenceIndex { get; }
        }
    }
}
