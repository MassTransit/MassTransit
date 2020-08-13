namespace MassTransit.Testing
{
    using Decorators;
    using MessageObservers;


    public class ConsumerTestHarness<TConsumer> :
        IConsumerTestHarness<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly ReceivedMessageList _consumed;
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory, string queueName)
            : this(testHarness, consumerFactory)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
            else
                testHarness.OnConfigureBus += configurator => ConfigureNamedReceiveEndpoint(configurator, queueName);
        }

        public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;

            _consumed = new ReceivedMessageList(testHarness.TestTimeout, testHarness.InactivityToken);
        }

        public IReceivedMessageList Consumed => _consumed;

        protected virtual void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            var decorator = new TestConsumerFactoryDecorator<TConsumer>(_consumerFactory, _consumed);

            configurator.Consumer(decorator);
        }

        protected virtual void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                var decorator = new TestConsumerFactoryDecorator<TConsumer>(_consumerFactory, _consumed);

                x.Consumer(decorator);
            });
        }
    }
}
