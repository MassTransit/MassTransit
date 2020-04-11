namespace MassTransit.PrometheusIntegration.Observers
{
    using Configuration;
    using EndpointConfigurators;


    public class PrometheusReceiveEndpointConfiguratorObserver :
        IEndpointConfigurationObserver
    {
        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            var specification = new PrometheusReceiveSpecification();

            configurator.ConfigureReceive(r => r.AddPipeSpecification(specification));

            configurator.ConnectReceiveEndpointObserver(new PrometheusReceiveEndpointObserver());
        }
    }
}
