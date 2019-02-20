namespace MassTransit.PipeConfigurators
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Configurators;


    /// <summary>
    /// Configures scheduled message redelivery for a consumer, on the consumer configurator, which is constrained to
    /// the message types for that consumer, and only applies to the consumer prior to the consumer factory.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class MessageRedeliveryConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;
        readonly Action<IRetryConfigurator> _configure;

        public MessageRedeliveryConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, Action<IRetryConfigurator> configure)
        {
            _configurator = configurator;
            _configure = configure;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var redeliverySpecification = new ScheduleMessageRedeliveryPipeSpecification<TMessage>();
            var retrySpecification = new RedeliveryRetryPipeSpecification<TMessage>();

            _configure?.Invoke(retrySpecification);

            _configurator.Message<TMessage>(x =>
            {
                x.AddPipeSpecification(redeliverySpecification);
                x.AddPipeSpecification(retrySpecification);
            });
        }
    }
}
