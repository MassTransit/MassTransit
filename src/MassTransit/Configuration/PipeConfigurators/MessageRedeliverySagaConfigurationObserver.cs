namespace MassTransit.PipeConfigurators
{
    using System;
    using GreenPipes.Configurators;
    using Saga;
    using SagaConfigurators;


    /// <summary>
    /// Configures scheduled message redelivery for a saga, on the saga configurator, which is constrained to
    /// the message types for that saga, and only applies to the saga prior to the saga repository.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public class MessageRedeliverySagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly ISagaConfigurator<TSaga> _configurator;
        readonly Action<IRetryConfigurator> _configure;

        public MessageRedeliverySagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, Action<IRetryConfigurator> configure)
        {
            _configurator = configurator;
            _configure = configure;
        }

        void ISagaConfigurationObserver.SagaConfigured<T>(ISagaConfigurator<T> configurator)
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<T, TMessage>(ISagaMessageConfigurator<T, TMessage> configurator)
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