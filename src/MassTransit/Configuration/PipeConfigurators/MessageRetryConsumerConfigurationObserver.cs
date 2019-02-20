namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Threading;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;


    /// <summary>
    /// Configures a message retry for a consumer, on the consumer configurator, which is constrained to
    /// the message types for that consumer, and only applies to the consumer prior to the consumer factory.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class MessageRetryConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;
        readonly CancellationToken _cancellationToken;
        readonly Action<IRetryConfigurator> _configure;

        public MessageRetryConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, CancellationToken cancellationToken,
            Action<IRetryConfigurator> configure)
        {
            _configurator = configurator;
            _cancellationToken = cancellationToken;
            _configure = configure;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(Factory, _cancellationToken);

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }

        static RetryConsumeContext<TMessage> Factory<TMessage>(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where TMessage : class
        {
            return new RetryConsumeContext<TMessage>(context, retryPolicy, retryContext);
        }
    }
}
