namespace MassTransit.Configuration
{
    using System;


    public class ConfigureReceiveEndpointDelegateProvider :
        IConfigureReceiveEndpoint
    {
        readonly ConfigureEndpointsProviderCallback _callback;
        readonly IRegistrationContext _context;

        public ConfigureReceiveEndpointDelegateProvider(IRegistrationContext context, ConfigureEndpointsProviderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _context = context;
            _callback = callback;
        }

        public void Configure(string name, IReceiveEndpointConfigurator configurator)
        {
            _callback(_context, name, configurator);
        }
    }
}
