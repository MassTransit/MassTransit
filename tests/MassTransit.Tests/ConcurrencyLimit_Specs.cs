namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class Using_a_consumer_concurrency_limit :
        InMemoryTestFixture
    {
        static readonly int _messageCount = 50;
        static TaskCompletionSource<bool> _complete;

        Consumer _consumer;

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

            Assert.AreEqual(2, _consumer.MaxDeliveryCount);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new Consumer();

            configurator.Consumer(() => _consumer, x => x.ConcurrentMessageLimit = 2);
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


    public class Using_a_saga_concurrency_limit :
        InMemoryTestFixture
    {
        static readonly int _messageCount = 50;
        static TaskCompletionSource<bool> _complete;
        ISagaRepository<ConsumerSaga> _repository;

        [Test]
        public async Task Should_limit_the_saga()
        {
            _complete = GetTask<bool>();

            var tasks = new List<Task>(_messageCount * 2);

            for (var i = 0; i < _messageCount; i++)
            {
                var sagaId = NewId.NextGuid();

                tasks.Add(Bus.Publish(new A(sagaId)));
                tasks.Add(Bus.Publish(new B(sagaId)));
            }

            await Task.WhenAll(tasks);

            await _complete.Task;

            Assert.AreEqual(2, ConsumerSaga.MaxDeliveryCount);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = new InMemorySagaRepository<ConsumerSaga>();

            configurator.Saga(_repository, x => x.ConcurrentMessageLimit = 2);
        }


        class ConsumerSaga :
            InitiatedByOrOrchestrates<A>,
            InitiatedByOrOrchestrates<B>,
            ISaga
        {
            static int _currentPendingDeliveryCount;
            static long _deliveryCount;
            static int _maxPendingDeliveryCount;

            public static int MaxDeliveryCount => _maxPendingDeliveryCount;

            public Task Consume(ConsumeContext<A> context)
            {
                return OnConsume();
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return OnConsume();
            }

            public Guid CorrelationId { get; set; }

            static async Task OnConsume()
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


        [Serializable]
        class A : CorrelatedBy<Guid>
        {
            public A(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        [Serializable]
        class B : CorrelatedBy<Guid>
        {
            public B(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }
    }
}
