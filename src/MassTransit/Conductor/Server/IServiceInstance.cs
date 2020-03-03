namespace MassTransit.Conductor.Server
{
    using System;


    public interface IServiceInstance
    {
        Guid InstanceId { get; }
        string InstanceName { get; }

        IServiceEndpoint CreateServiceEndpoint(IReceiveEndpointConfigurator configurator);
    }
}
