namespace MassTransit.Tests.Pipeline
{
    using Consumer;
    using MassTransit.Configuration;
    using MassTransit.Testing;
    using MassTransit.Transports;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class MessageTestFixture :
        AsyncTestFixture
    {
        public MessageTestFixture()
            : base(new InMemoryTestHarness())
        {
        }

        protected ConsumeContext GetConsumeContext<T>(T message)
            where T : class
        {
            return new TestConsumeContext<T>(message);
        }

        protected OneMessageConsumer GetOneMessageConsumer()
        {
            return new OneMessageConsumer(GetTask<MessageA>());
        }

        protected TwoMessageConsumer GetTwoMessageConsumer()
        {
            return new TwoMessageConsumer(GetTask<MessageA>(), GetTask<MessageB>());
        }

        protected IConsumerFactory<T> GetInstanceConsumerFactory<T>(T consumer)
            where T : class
        {
            return new InstanceConsumerFactory<T>(consumer);
        }

        protected IConsumePipe CreateConsumePipe()
        {
            var builder = new ConsumePipeSpecification();

            return builder.BuildConsumePipe();
        }
    }
}
