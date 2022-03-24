namespace MassTransit.Monitoring.Configuration
{
    public class InstrumentHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        public void HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
            where T : class
        {
            var specification = new InstrumentHandlerSpecification<T>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
