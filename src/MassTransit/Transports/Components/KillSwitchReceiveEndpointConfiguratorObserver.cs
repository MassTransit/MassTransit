namespace MassTransit.Transports.Components
{
    using EndpointConfigurators;


    public class KillSwitchReceiveEndpointConfiguratorObserver :
        IEndpointConfigurationObserver
    {
        readonly KillSwitchReceiveEndpointObserver _observer;

        public KillSwitchReceiveEndpointConfiguratorObserver(KillSwitchOptions options)
        {
            _observer = new KillSwitchReceiveEndpointObserver(options);
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            configurator.ConnectReceiveEndpointObserver(_observer);
        }
    }
}
