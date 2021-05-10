namespace MassTransit
{
    public delegate void ConfigureEndpointsCallback(string queueName, IReceiveEndpointConfigurator configurator);
}
