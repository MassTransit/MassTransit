namespace MassTransit.Monitoring.Configuration
{
    public class InstrumentReceiveEndpointConfiguratorObserver :
        IEndpointConfigurationObserver
    {
        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            var specification = new InstrumentReceiveSpecification();

            configurator.ConfigureReceive(r => r.AddPipeSpecification(specification));
        }
    }
}
