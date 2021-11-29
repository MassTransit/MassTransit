namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using MassTransit.Testing.Implementations;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Connecting_an_observer :
        MessageTestFixture
    {
        [Test]
        public void Should_invoke_faulted()
        {
            var filter = CreateConsumePipe();

            filter.ConnectHandler<MessageA>(async context =>
            {
                throw new IntentionalTestException("This is a test");
            });

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            var consumeContext = GetConsumeContext(new MessageA());

            Assert.That(async () => await filter.Send(consumeContext), Throws.TypeOf<IntentionalTestException>());

            Assert.That(async () => await interceptor.ConsumeFaulted, Throws.TypeOf<IntentionalTestException>());
        }

        [Test]
        public async Task Should_invoke_post()
        {
            var filter = CreateConsumePipe();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectHandler<MessageA>(async context => received.TrySetResult(context.Message));

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            var consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PostConsumed;
        }

        [Test]
        public async Task Should_invoke_post_consumer()
        {
            var filter = CreateConsumePipe();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectConsumer(() => new OneMessageConsumer(received));

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            var consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PostConsumed;
        }

        [Test]
        public async Task Should_invoke_pre()
        {
            var filter = CreateConsumePipe();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectHandler<MessageA>(async context => received.TrySetResult(context.Message));

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            var consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PreConsumed;
        }
    }
}
