namespace MassTransit.RabbitMqTransport.Tests
{
    namespace ConsumerBind_Specs
    {
        using System;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using NUnit.Framework;
        using RabbitMQ.Client;
        using Util;


        public class ConsumerBindingTestFixture :
            RabbitMqTestFixture
        {
            protected override void OnCleanupVirtualHost(IModel model)
            {
                model.ExchangeDelete(NameFormatter.GetMessageName(typeof(A)).ToString());
                model.ExchangeDelete(NameFormatter.GetMessageName(typeof(B)).ToString());
            }
        }


        [TestFixture]
        public class Binding_an_untyped_consumer :
            ConsumerBindingTestFixture
        {
            [Test]
            [Order(0)]
            public async Task Setup()
            {
                await InputQueueSendEndpoint.Send(new A());
                await InputQueueSendEndpoint.Send(new B());
            }

            [Test]
            public async Task Should_receive_the_message_a()
            {
                await _testConsumer.A.Task;
            }

            [Test]
            public async Task Should_receive_the_message_b()
            {
                await _testConsumer.B.Task;
            }

            TestConsumer _testConsumer;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                _testConsumer = new TestConsumer(GetTask<A>(), GetTask<B>());

                configurator.Consumer(typeof(TestConsumer), type =>
                {
                    return _testConsumer;
                });
            }
        }


        [TestFixture]
        public class Binding_a_typed_consumer :
            ConsumerBindingTestFixture
        {
            [Test]
            public async Task Should_receive_the_message_a()
            {
                await _testConsumer.A.Task;
            }

            [Test]
            public async Task Should_receive_the_message_b()
            {
                await _testConsumer.B.Task;
            }

            TestConsumer _testConsumer;

            [OneTimeSetUp]
            public async Task Setup()
            {
                await InputQueueSendEndpoint.Send(new A());
                await InputQueueSendEndpoint.Send(new B());
            }

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                _testConsumer = new TestConsumer(GetTask<A>(), GetTask<B>());

                configurator.Consumer(() => _testConsumer);
            }
        }


        [TestFixture]
        public class Binding_a_handler :
            ConsumerBindingTestFixture
        {
            [Test]
            public async Task Should_receive_the_message_a()
            {
                await _a;
            }

            [Test]
            public async Task Should_receive_the_message_b()
            {
                await _b;
            }

            Task<ConsumeContext<A>> _a;
            Task<ConsumeContext<B>> _b;

            [OneTimeSetUp]
            public async Task Setup()
            {
                await InputQueueSendEndpoint.Send(new A());
                await InputQueueSendEndpoint.Send(new B());
            }

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                _a = Handled<A>(configurator);
                _b = Handled<B>(configurator);
            }
        }


        [TestFixture]
        public class Configuring_a_consumer_without_binding :
            ConsumerBindingTestFixture
        {
            [Test]
            public async Task Should_receive_the_message_a()
            {
                await _a;
            }

            [Test]
            public async Task Should_receive_the_message_b()
            {
                await _b;
            }

            Task<ConsumeContext<A>> _a;
            Task<ConsumeContext<B>> _b;

            [OneTimeSetUp]
            public async Task Setup()
            {
                await InputQueueSendEndpoint.Send(new A());
                await InputQueueSendEndpoint.Send(new B());
            }

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                configurator.ConfigureConsumeTopology = false;

                _a = Handled<A>(configurator);
                _b = Handled<B>(configurator);
            }
        }


        [TestFixture]
        public class Binding_a_old_school_saga :
            ConsumerBindingTestFixture
        {
            [Test]
            public async Task Should_receive_the_message_a()
            {
                Guid? sagaId = await _repository.ShouldContainSaga(_sagaId, TestTimeout);
                Assert.IsTrue(sagaId.HasValue);

                var saga = _repository[sagaId.Value].Instance;

                await saga.A.Task;
            }

            [Test]
            public async Task Should_receive_the_message_b()
            {
                Guid? sagaId = await _repository.ShouldContainSaga(_sagaId, TestTimeout);
                Assert.IsTrue(sagaId.HasValue);

                var saga = _repository[sagaId.Value].Instance;

                await InputQueueSendEndpoint.Send(new B { CorrelationId = _sagaId });

                await saga.B.Task;
            }

            InMemorySagaRepository<TestSaga> _repository;
            Guid _sagaId;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _sagaId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new A { CorrelationId = _sagaId });
            }

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                _repository = new InMemorySagaRepository<TestSaga>();

                configurator.Saga(_repository);
            }
        }


        class TestConsumer :
            IConsumer<A>,
            IConsumer<B>
        {
            public TestConsumer(TaskCompletionSource<A> a, TaskCompletionSource<B> b)
            {
                A = a;
                B = b;
            }

            public TaskCompletionSource<A> A { get; }

            public TaskCompletionSource<B> B { get; }

            public async Task Consume(ConsumeContext<A> context)
            {
                A.TrySetResult(context.Message);
            }

            public async Task Consume(ConsumeContext<B> context)
            {
                B.TrySetResult(context.Message);
            }
        }


        public class TestSaga :
            ISaga,
            InitiatedBy<A>,
            Orchestrates<B>
        {
            public TestSaga()
            {
            }

            public TestSaga(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public TaskCompletionSource<A> A { get; } = TaskUtil.GetTask<A>();

            public TaskCompletionSource<B> B { get; } = TaskUtil.GetTask<B>();

            public async Task Consume(ConsumeContext<A> context)
            {
                A.TrySetResult(context.Message);
            }

            public Guid CorrelationId { get; set; }

            public async Task Consume(ConsumeContext<B> context)
            {
                B.TrySetResult(context.Message);
            }
        }


        public class A :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class B :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
