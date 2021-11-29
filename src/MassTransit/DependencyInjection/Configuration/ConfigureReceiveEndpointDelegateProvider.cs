namespace MassTransit.Configuration
{
    using System;


    public class ConfigureReceiveEndpointDelegateProvider :
        IConfigureReceiveEndpoint
    {
        readonly IServiceProvider _provider;
        readonly ConfigureEndpointsProviderCallback _callback;

        public ConfigureReceiveEndpointDelegateProvider(IServiceProvider provider, ConfigureEndpointsProviderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _provider = provider;
            _callback = callback;
        }

        public void Configure(string name, IReceiveEndpointConfigurator configurator)
        {
            _callback(_provider, name, configurator);
        }
    }
}
