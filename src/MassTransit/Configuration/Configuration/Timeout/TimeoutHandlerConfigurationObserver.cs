namespace MassTransit.Configuration
{
    using System;


    public class TimeoutHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        readonly Action<ITimeoutConfigurator> _configure;

        public TimeoutHandlerConfigurationObserver(Action<ITimeoutConfigurator> configure)
        {
            _configure = configure;
        }

        void IHandlerConfigurationObserver.HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
        {
            var specification = new TimeoutSpecification<T>();

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}
