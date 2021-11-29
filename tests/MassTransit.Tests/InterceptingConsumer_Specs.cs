namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class Intercepting_a_consumer_factory :
        InMemoryTestFixture
    {
        MyConsumer _myConsumer;
        TransactionFilter _transactionFilter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _myConsumer = new MyConsumer(GetTask<A>());
            _transactionFilter = new TransactionFilter(GetTask<bool>(), GetTask<bool>());

            configurator.Consumer(() => _myConsumer, x => x.UseFilter(_transactionFilter));
        }

        [Test]
        public async Task Should_call_the_consumer_method()
        {
            await _myConsumer.Called.Task;
        }

        [Test]
        public async Task Should_call_the_interceptor_first()
        {
            await _transactionFilter.First.Task;
        }

        [Test]
        public async Task Should_call_the_interceptor_second()
        {
            await _transactionFilter.Second.Task;
        }


        class TransactionFilter :
            IFilter<ConsumerConsumeContext<MyConsumer>>
        {
            public readonly TaskCompletionSource<bool> First;
            public readonly TaskCompletionSource<bool> Second;

            public TransactionFilter(TaskCompletionSource<bool> first, TaskCompletionSource<bool> second)
            {
                First = first;
                Second = second;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            public async Task Send(ConsumerConsumeContext<MyConsumer> context, IPipe<ConsumerConsumeContext<MyConsumer>> next)
            {
                First.TrySetResult(true);

                await next.Send(context);

                Second.TrySetResult(true);
            }
        }


        class MyConsumer :
            IConsumer<A>
        {
            public readonly TaskCompletionSource<A> Called;

            public MyConsumer(TaskCompletionSource<A> called)
            {
                Called = called;
            }

            public async Task Consume(ConsumeContext<A> message)
            {
                Called.TrySetResult(message.Message);
            }
        }


        class A
        {
        }
    }
}
