namespace MassTransit.PipeConfigurators
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Configurators;


    /// <summary>
    /// Configures a message retry for a handler, on the handler configurator, which is constrained to
    /// the message types for that handler, and only applies to the handler.
    /// </summary>
    public class MessageRedeliveryHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        readonly Action<IRetryConfigurator> _configure;

        public MessageRedeliveryHandlerConfigurationObserver(Action<IRetryConfigurator> configure)
        {
            _configure = configure;
        }

        void IHandlerConfigurationObserver.HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
        {
            var redeliverySpecification = new ScheduleMessageRedeliveryPipeSpecification<T>();
            var retrySpecification = new RedeliveryRetryPipeSpecification<T>();

            _configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(redeliverySpecification);
            configurator.AddPipeSpecification(retrySpecification);
        }
    }
}
