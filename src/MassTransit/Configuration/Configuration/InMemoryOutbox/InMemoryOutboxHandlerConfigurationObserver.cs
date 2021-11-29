namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Configures a message retry for a handler, on the handler configurator, which is constrained to
    /// the message types for that handler, and only applies to the handler.
    /// </summary>
    public class InMemoryOutboxHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        readonly Action<IOutboxConfigurator> _configure;

        public InMemoryOutboxHandlerConfigurationObserver(Action<IOutboxConfigurator> configure)
        {
            _configure = configure;
        }

        void IHandlerConfigurationObserver.HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
        {
            var specification = new InMemoryOutboxSpecification<T>();

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}
