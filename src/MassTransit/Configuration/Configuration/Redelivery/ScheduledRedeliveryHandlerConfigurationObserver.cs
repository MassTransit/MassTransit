namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Configures a message retry for a handler, on the handler configurator, which is constrained to
    /// the message types for that handler, and only applies to the handler.
    /// </summary>
    public class ScheduledRedeliveryHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        readonly Action<IRetryConfigurator> _configure;

        public ScheduledRedeliveryHandlerConfigurationObserver(Action<IRetryConfigurator> configure)
        {
            _configure = configure;
        }

        void IHandlerConfigurationObserver.HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
        {
            var redeliverySpecification = new ScheduledRedeliveryPipeSpecification<T>();
            var retrySpecification = new RedeliveryRetryPipeSpecification<T>(redeliverySpecification);

            _configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(redeliverySpecification);
            configurator.AddPipeSpecification(retrySpecification);
        }
    }
}
