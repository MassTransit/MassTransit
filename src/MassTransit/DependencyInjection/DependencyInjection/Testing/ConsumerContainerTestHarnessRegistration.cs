namespace MassTransit.DependencyInjection.Testing
{
    using MassTransit.Testing;
    using MassTransit.Testing.Implementations;
    using Registration;


    public class ConsumerContainerTestHarnessRegistration<TConsumer> :
        IConsumerFactoryDecoratorRegistration<TConsumer>
        where TConsumer : class, IConsumer
    {
        public ConsumerContainerTestHarnessRegistration(ITestHarness testHarness)
        {
            Consumed = new ReceivedMessageList(testHarness.TestTimeout, testHarness.InactivityToken);
        }

        public ReceivedMessageList Consumed { get; }

        public IConsumerFactory<TConsumer> DecorateConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
        {
            return new TestConsumerFactoryDecorator<TConsumer>(consumerFactory, Consumed);
        }
    }
}
