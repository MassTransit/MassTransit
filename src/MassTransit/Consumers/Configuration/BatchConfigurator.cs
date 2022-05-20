#nullable enable
namespace MassTransit.Configuration
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

            ConcurrencyLimit = 1;
            MessageLimit = 10;
            TimeLimit = TimeSpan.FromSeconds(10);
        }

        public TimeSpan TimeLimit { private get; set; }
        public int MessageLimit { private get; set; }
        public int ConcurrencyLimit { private get; set; }

        public void Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>>? configure)
            where TConsumer : class, IConsumer<Batch<TMessage>>
        {
            var configurator = new ConsumerConfigurator<TConsumer>(consumerFactory, _configurator);
            configurator.ConnectConsumerConfigurationObserver(_configurator);

            configurator.Options<BatchOptions>(options => options.SetMessageLimit(MessageLimit).SetTimeLimit(TimeLimit).SetConcurrencyLimit(ConcurrencyLimit));

            configurator.ConsumerMessage(configure);

            _configurator.AddEndpointSpecification(configurator);
        }
    }
}
