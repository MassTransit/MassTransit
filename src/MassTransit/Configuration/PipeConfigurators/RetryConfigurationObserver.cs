namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Threading;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;


    public class RetryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly CancellationToken _cancellationToken;
        readonly Action<IRetryConfigurator> _configure;

        public RetryConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, CancellationToken cancellationToken,
            Action<IRetryConfigurator> configure)
            : base(receiveEndpointConfigurator)
        {
            _cancellationToken = cancellationToken;
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(Factory, _cancellationToken);

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        static RetryConsumeContext<TMessage> Factory<TMessage>(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where TMessage : class
        {
            return new RetryConsumeContext<TMessage>(context, retryPolicy, retryContext);
        }
    }
}
