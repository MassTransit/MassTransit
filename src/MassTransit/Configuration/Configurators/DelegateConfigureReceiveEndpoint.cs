namespace MassTransit.Configurators
{
    using System;


    public class DelegateConfigureReceiveEndpoint :
        IConfigureReceiveEndpoint
    {
        readonly Action<string, IReceiveEndpointConfigurator> _configure;

        public DelegateConfigureReceiveEndpoint(Action<string, IReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            _configure = configure;
        }

        public void Configure(string name, IReceiveEndpointConfigurator configurator)
        {
            _configure(name, configurator);
        }
    }
}
