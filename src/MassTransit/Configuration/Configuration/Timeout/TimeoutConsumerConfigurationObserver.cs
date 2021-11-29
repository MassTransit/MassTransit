namespace MassTransit.Configuration
{
    using System;


    public class TimeoutConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;
        readonly Action<ITimeoutConfigurator> _configure;

        public TimeoutConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, Action<ITimeoutConfigurator> configure)
        {
            _configurator = configurator;
            _configure = configure;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new TimeoutSpecification<TMessage>();

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
