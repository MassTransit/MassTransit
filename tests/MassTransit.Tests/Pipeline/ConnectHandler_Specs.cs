namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Connecting_a_handler_to_the_inbound_pipe :
        MessageTestFixture
    {
        [Test]
        public async Task Should_receive_a_test_message()
        {
            var filter = CreateConsumePipe();

            TaskCompletionSource<PingMessage> received = GetTask<PingMessage>();

            var connectHandle = filter.ConnectHandler<PingMessage>(async context =>
                received.TrySetResult(context.Message));

            var consumeContext = new TestConsumeContext<PingMessage>(new PingMessage());

            await filter.Send(consumeContext);

            await received.Task;
        }
    }
}
