namespace MassTransit.Configuration
{
    using System;


    public class InMemoryOutboxConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;
        readonly Action<IOutboxConfigurator> _configure;
        readonly ISetScopedConsumeContext _setter;

        public InMemoryOutboxConsumerConfigurationObserver(IRegistrationContext context, IConsumerConfigurator<TConsumer> configurator,
            Action<IOutboxConfigurator> configure)
            : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)), configurator, configure)
        {
        }

        public InMemoryOutboxConsumerConfigurationObserver(ISetScopedConsumeContext setter, IConsumerConfigurator<TConsumer> configurator,
            Action<IOutboxConfigurator> configure)
        {
            _setter = setter;
            _configurator = configurator;
            _configure = configure;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new InMemoryOutboxSpecification<TMessage>(_setter);

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
