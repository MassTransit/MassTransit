namespace MassTransit
{
    public delegate void ConfigureEndpointsProviderCallback(IRegistrationContext context, string queueName, IReceiveEndpointConfigurator configurator);
}
