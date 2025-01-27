namespace MassTransit;

public delegate void SqlConfigureEndpointsCallback(IRegistrationContext context, string queueName, ISqlReceiveEndpointConfigurator configurator);
