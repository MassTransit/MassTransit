namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Connecting_a_consumer_to_the_inbound_pipe :
        MessageTestFixture
    {
        [Test]
        public async Task Should_receive_a_message()
        {
            var filter = CreateConsumePipe();

            var consumer = GetOneMessageConsumer();

            IConsumerFactory<OneMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            filter.ConnectConsumer(factory);

            var consumeContext = new TestConsumeContext<MessageA>(new MessageA());

            await filter.Send(consumeContext);

            await consumer.Task;
        }

        [Test]
        [Explicit]
        public void Should_receive_a_message_pipeline_view()
        {
            var filter = CreateConsumePipe();

            var consumer = GetOneMessageConsumer();

            IConsumerFactory<OneMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            filter.ConnectConsumer(factory);
        }

        [Test]
        public async Task Should_receive_a_message_via_object()
        {
            var filter = CreateConsumePipe();

            var consumer = GetOneMessageConsumer();

            object subscribeConsumer = consumer;

            filter.ConnectInstance(subscribeConsumer);

            var consumeContext = new TestConsumeContext<MessageA>(new MessageA());

            await filter.Send(consumeContext);

            await consumer.Task;
        }

        [Test]
        public async Task Should_receive_a_two_messages()
        {
            var filter = CreateConsumePipe();

            var consumer = GetTwoMessageConsumer();

            IConsumerFactory<TwoMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            filter.ConnectConsumer(factory);

            await filter.Send(new TestConsumeContext<MessageA>(new MessageA()));

            await filter.Send(new TestConsumeContext<MessageB>(new MessageB()));

            await consumer.TaskA;
            await consumer.TaskB;
        }
    }
}
