namespace MassTransit.Configurators
{
    using System;


    public class DelegateConfigureReceiveEndpoint :
        IConfigureReceiveEndpoint
    {
        readonly ConfigureEndpointsCallback _callback;

        public DelegateConfigureReceiveEndpoint(ConfigureEndpointsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _callback = callback;
        }

        public void Configure(string name, IReceiveEndpointConfigurator configurator)
        {
            _callback(name, configurator);
        }
    }
}
