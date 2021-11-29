namespace MassTransit.Configuration
{
    using System;
    using System.Threading;
    using RetryPolicies;


    /// <summary>
    /// Configures a message retry for a handler, on the handler configurator, which is constrained to
    /// the message types for that handler, and only applies to the handler.
    /// </summary>
    public class MessageRetryHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        readonly CancellationToken _cancellationToken;
        readonly Action<IRetryConfigurator> _configure;

        public MessageRetryHandlerConfigurationObserver(CancellationToken cancellationToken,
            Action<IRetryConfigurator> configure)
        {
            _cancellationToken = cancellationToken;
            _configure = configure;
        }

        void IHandlerConfigurationObserver.HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
        {
            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<T>, RetryConsumeContext<T>>(Factory, _cancellationToken);

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        static RetryConsumeContext<T> Factory<T>(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where T : class
        {
            return new RetryConsumeContext<T>(context, retryPolicy, retryContext);
        }
    }
}
