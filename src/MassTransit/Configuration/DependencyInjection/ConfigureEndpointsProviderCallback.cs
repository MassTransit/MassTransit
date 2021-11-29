namespace MassTransit
{
    using System;


    public delegate void ConfigureEndpointsProviderCallback(IServiceProvider context, string queueName, IReceiveEndpointConfigurator configurator);
}
