namespace MassTransit.ConsumeConfigurators
{
    using System;


    public class BatchConfigurator<TMessage> :
        IBatchConfigurator<TMessage>
        where TMessage : class
    {
        readonly IReceiveEndpointConfigurator _configurator;

        public BatchConfigurator(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;

            MessageLimit = 10;
            TimeLimit = TimeSpan.FromSeconds(10);
        }

        public TimeSpan TimeLimit { private get; set; }
        public int MessageLimit { private get; set; }

        void IBatchConfigurator<TMessage>.Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure)
        {
            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, _configurator);
            consumerConfigurator.ConnectConsumerConfigurationObserver(_configurator);

            consumerConfigurator.Options<BatchOptions>(options => options.SetMessageLimit(MessageLimit).SetTimeLimit(TimeLimit));

            consumerConfigurator.ConsumerMessage(configure);

            _configurator.AddEndpointSpecification(consumerConfigurator);
        }
    }
}
